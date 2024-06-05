using Flex.LVA.Shared;

namespace Flex.LVA.Core
{
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Logging;
    using Flex.LVA.Shared.Containers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Eventing.Reader;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Tracer = Logging.Tracer;

    internal class EventColumnConsts
    {
        internal const string DateTime = "Timestamp";
        internal const string Source = "Source";
        internal const string EventId = "Event ID";
        internal const string Message = "Message";
        internal const string TaskCat = "Task Category";
        internal const string Level = "Severity";
    }

    internal class EventLogParser : ILogParser, IRawLogStore
    {
        private readonly Domain myDomain;

        public event EventHandler<ILogHeader> HeaderReady;
        public event EventHandler<ProgressData> LogChunkReady;
        public event EventHandler JobFinished;
        private IDictionary<int, EventRecord> myRawLogs = new Dictionary<int, EventRecord>();
        private LogGraphMutable myLogGraph;
        private Stream myEventFile;
        public ILogGraph LogGraph { get; private set; }

        public IRawLogStore LogStore => this;

        public Stream Writer => throw new NotImplementedException();

        private readonly string myEvtxPath;
        public EventLogParser(long theEngineId)
        {
            myDomain = new Domain($"{typeof(JSONLogParser).FullName}.{theEngineId}");
            myEvtxPath = Path.Combine(FlexDirectories.LvaTempDir, $"{theEngineId}.evtx");
            this.Reset();
        }

        public void Parse(LogDefinition theLogDefinition, in string theStringToParse = null)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                FreeUpFileStream();
                LogContainerFactory aFac = new LogContainerFactory();
                this.PrepareHeaderForEvent();
                HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                using (var aReader = new EventLogReader(myEvtxPath, PathType.FilePath))
                {
                    EventRecord aRecord = aReader.ReadEvent();
                    if (aRecord == null)
                    {
                        aTracer.Warn("No elements found in XML Array");
                        JobFinished?.Invoke(this, EventArgs.Empty);
                    }

                    int aLogId = 1;
                    int aLogStartId = aLogId;
                    int aTotalElements = int.MaxValue;
                    float aPreviousPercent = 0.0f;
                    Dictionary<byte, string> aEnumMap = new Dictionary<byte, string>()
                    {
                        {1, "Critical" },
                        {2, "Error" },
                        {4, "Informational" },
                        {0, "LogAlways" },
                        {5, "Verbose" },
                        {3, "Warning" },
                    };
                    try
                    {
                        while (true)
                        {
                            List<object> aList = new List<object>(this.myLogGraph.GraphHeader.ColumnNames.Count);
                            this.myLogGraph.SemanticLogs[aLogId] = aList;
                            aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Header), ParserUtils.GetHeader(aFac, true, this.myLogGraph));
                            aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Id), aFac.GetLogContainer(LogElementType.Number, aLogId, null));
                            aList.Insert(this.myLogGraph.GraphHeader.Index(EventColumnConsts.Level), ParserUtils.GetLogColumnString(aFac, aEnumMap[aRecord.Level.GetValueOrDefault()], EventColumnConsts.Level, myLogGraph));
                            aList.Insert(
                                this.myLogGraph.GraphHeader.Index(EventColumnConsts.DateTime),
                                ParserUtils.GetLogColumn(aFac, aRecord.TimeCreated.GetValueOrDefault().ToString(Constants.DateTimeFormat), EventColumnConsts.DateTime, myLogGraph, Constants.DateTimeFormat, LogElementType.DateTime));
                            aList.Insert(this.myLogGraph.GraphHeader.Index(EventColumnConsts.Source), ParserUtils.GetLogColumnString(aFac, aRecord.ProviderName, EventColumnConsts.Source, myLogGraph));
                            aList.Insert(this.myLogGraph.GraphHeader.Index(EventColumnConsts.EventId), ParserUtils.GetLogColumnString(aFac, aRecord.Id.ToString(), EventColumnConsts.EventId, myLogGraph));
                            aList.Insert(this.myLogGraph.GraphHeader.Index(EventColumnConsts.TaskCat), ParserUtils.GetLogColumnString(aFac, GetSafeDisplayName(aRecord), EventColumnConsts.TaskCat, myLogGraph));
                            string aDec = aRecord.FormatDescription();
                            if (aRecord.FormatDescription() == null)
                            {
                                aDec = "Description not available";
                            }

                            aList.Insert(this.myLogGraph.GraphHeader.Index(EventColumnConsts.Message), ParserUtils.GetLogColumnString(aFac, aDec, EventColumnConsts.Message, myLogGraph));
                            this.myRawLogs.Add(aLogId, aRecord);
                            aRecord = aReader.ReadEvent();
                            if (aRecord == null)
                            {
                                break;
                            }

                            aLogId++;
                        }
                    }
                    catch (Exception aEx)
                    {
                        aTracer.Error($"Encountered error when parsing event logs {aEx}");
                    }

                    HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                    LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, aLogId), aLogId, aTotalElements));
                    JobFinished?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string GetSafeDisplayName(EventRecord theRecord)
        {
            try
            {
                if(theRecord.TaskDisplayName == null)
                {
                    return "None";
                }

                return theRecord.TaskDisplayName;
            }
            catch (EventLogNotFoundException)
            {
                return "None";
            }
        }
        private void FreeUpFileStream()
        {
            this.myEventFile?.Close();
            this.myEventFile?.Dispose();
            this.myEventFile = null;
        }

        private void PrepareHeaderForEvent()
        {
            this.myLogGraph.GraphHeader.Add(Constants.Header, LogElementType.Bool);
            this.myLogGraph.GraphHeader.Add(Constants.Id, LogElementType.Number);
            this.myLogGraph.GraphHeader.Add(EventColumnConsts.Level, LogElementType.String);
            this.myLogGraph.GraphHeader.Add(EventColumnConsts.DateTime, LogElementType.DateTime);
            this.myLogGraph.GraphHeader.Add(EventColumnConsts.Source, LogElementType.String);
            this.myLogGraph.GraphHeader.Add(EventColumnConsts.EventId, LogElementType.Number);
            this.myLogGraph.GraphHeader.Add(EventColumnConsts.TaskCat, LogElementType.String);
            this.myLogGraph.GraphHeader.Add(EventColumnConsts.Message, LogElementType.String);
            this.myLogGraph.Graph[Constants.Header] = new Dictionary<string, object>();
            this.myLogGraph.Graph[Constants.Id] = new Dictionary<string, object>();
            this.myLogGraph.Graph[EventColumnConsts.Level] = new Dictionary<string, object>();
            this.myLogGraph.Graph[EventColumnConsts.DateTime] = new Dictionary<string, object>();
            this.myLogGraph.Graph[EventColumnConsts.Source] = new Dictionary<string, object>();
            this.myLogGraph.Graph[EventColumnConsts.EventId] = new Dictionary<string, object>();
            this.myLogGraph.Graph[EventColumnConsts.TaskCat] = new Dictionary<string, object>();
            this.myLogGraph.Graph[EventColumnConsts.Message] = new Dictionary<string, object>();
        }

        public void Reset()
        {
            FreeUpFileStream();
            myEventFile = new FileStream(myEvtxPath, FileMode.OpenOrCreate, FileAccess.Write);
            this.myRawLogs = new Dictionary<int, EventRecord>();
            FreeUpMemory();
        }

        public void FreeUpMemory()
        {
            myLogGraph = new LogGraphMutable();
            LogGraph = new LogGraph(myLogGraph.GraphHeader, this.myLogGraph.SemanticLogs);
        }

        public void AppendLogChunk(IEnumerable<byte> theChunk)
        {
            myEventFile.Write(theChunk.ToArray());
            myEventFile.Flush();
        }

        public void Dispose()
        {
            using (var aTracer = new Tracer(myDomain))
            {
                FreeUpFileStream();
                if (!File.Exists(myEvtxPath))
                {
                    aTracer.Debug($"File {myEvtxPath} Does Not Exist...");
                    return;
                }

                File.Delete(this.myEvtxPath);
                File.Delete(this.myEvtxPath);
                Thread.Sleep(1000);
                File.Delete(this.myEvtxPath);
                File.Delete(this.myEvtxPath);
                aTracer.Info($"File {myEvtxPath} Cleaned Up...");
            }
        }

        public bool UpdateRange(int theRangeId, int theEndIndex)
        {
            return true;
        }

        public bool StoreRange(int theRangeId, int theStartIndex, int theEndIndex)
        {
            return true;
        }

        public bool GetString(int theRangeId, out string theString)
        {
            theString = null;
            if (!this.myRawLogs.TryGetValue(theRangeId, out EventRecord aRec))
            {
                return false;
            }

            theString = XDocument.Parse(aRec.ToXml()).ToString();
            return true;
        }

        public void LoadStore(in string theFallbackString)
        {
            /// NOP
        }

        public void ResetStore()
        {
            throw new NotImplementedException();
        }
    }
}
