using Flex.LVA.Shared;

namespace Flex.LVA.Core
{
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Logging;
    using Flex.LVA.Shared.Containers;
    using SyslogDecode.Model;
    using SyslogDecode.Parsing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;
    using System.Xml.Linq;

    internal class SysLogParser : StringAndFileBuilder, ILogParser
    {
        public event EventHandler<ILogHeader> HeaderReady;
        public event EventHandler<ProgressData> LogChunkReady;
        public event EventHandler JobFinished;
        private IDictionary<int, string> myRawLogs = new Dictionary<int, string>();
        private LogGraphMutable myLogGraph;

        public ILogGraph LogGraph { get; private set; }

        public SysLogParser(long theEngineId) : base(theEngineId, new RawLogStreamer(theEngineId), typeof(SysLogParser))
        {
            this.Reset();
        }

        public void Parse(LogDefinition theLogDefinition, in string theStringToParse = null)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                Reset();
                string aBuffered;
                if (theStringToParse == null)
                {
                    aBuffered = myBuilder.ToString();
                }
                else
                {
                    aBuffered = theStringToParse;
                }

                if (theLogDefinition.HeaderLineCount.HasValue)
                {
                    aBuffered = ParserUtils.SkipLinesFromString(aBuffered, theLogDefinition.HeaderLineCount.Value);
                }

                SyslogMessageParser aParser = SyslogMessageParser.CreateDefault();
                LogContainerFactory aFac = new LogContainerFactory();
                Task.Run(this.ClearAllChunks).HandleException(myDomain, nameof(ClearAllChunks));
                Task.Run(() => this.LogStore.LoadStore(aBuffered)).HandleException(myDomain, nameof(RawLogStreamer.LoadStore));

                this.myLogGraph.GraphHeader.Add(Constants.Header, LogElementType.Bool);
                this.myLogGraph.GraphHeader.Add(Constants.Id, LogElementType.Number);
                this.myLogGraph.Graph[Constants.Header] = new Dictionary<string, object>();
                this.myLogGraph.Graph[Constants.Id] = new Dictionary<string, object>();
                int aStartIndex = 0;
                if (aStartIndex == -1)
                {
                    aTracer.Warn("No elements found in SysLog");
                    JobFinished?.Invoke(this, EventArgs.Empty);
                }

                int aNextIndex = 0;
                RawSyslogMessage aToParse = new RawSyslogMessage();
                int aLogId = 1;
                int aLogStartId = aLogId;
                int aTotalLength = aBuffered.Length;
                float aPreviousPercent = 0.0f;
                const string aColumnDate = "Date";
                const string aColumnTime = "Time";
                const string aColumnFacility = "Facility";
                const string aColumnHostname = "Hostname";
                const string aColumnAppName = "AppName";
                const string aColumnSeverity = "Severity";
                const string aColumnMessage = "Message";
                const string aColumnAdditionalData = "Additional Data";
                const string aDateFormat = "dd-MM-yyyy";
                const string aTimeFormat = "HH:mm:ss";

                this.myLogGraph.GraphHeader.Add(aColumnDate, LogElementType.Date);
                this.myLogGraph.GraphHeader.Add(aColumnTime, LogElementType.Time);
                this.myLogGraph.GraphHeader.Add(aColumnFacility, LogElementType.String);
                this.myLogGraph.GraphHeader.Add(aColumnHostname, LogElementType.String);
                this.myLogGraph.GraphHeader.Add(aColumnAppName, LogElementType.String);
                this.myLogGraph.GraphHeader.Add(aColumnSeverity, LogElementType.String);
                this.myLogGraph.GraphHeader.Add(aColumnMessage, LogElementType.String);
                this.myLogGraph.GraphHeader.Add(aColumnAdditionalData, LogElementType.String);

                this.myLogGraph.Graph[aColumnDate] = new Dictionary<string, object>();
                this.myLogGraph.Graph[aColumnTime] = new Dictionary<string, object>();
                this.myLogGraph.Graph[aColumnFacility] = new Dictionary<string, object>();
                this.myLogGraph.Graph[aColumnHostname] = new Dictionary<string, object>();
                this.myLogGraph.Graph[aColumnAppName] = new Dictionary<string, object>();
                this.myLogGraph.Graph[aColumnSeverity] = new Dictionary<string, object>();
                this.myLogGraph.Graph[aColumnMessage] = new Dictionary<string, object>();
                this.myLogGraph.Graph[aColumnAdditionalData] = new Dictionary<string, object>();
                HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                while (true)
                {
                    aNextIndex = aBuffered.IndexOf("\n", aStartIndex);
                    if (aNextIndex == -1)
                    {
                        aTracer.Warn("Parsing Finished...");
                        JobFinished?.Invoke(this, EventArgs.Empty);
                        break;
                    }

                    List<object> aList = new List<object>(this.myLogGraph.GraphHeader.ColumnNames.Count);
                    this.myLogGraph.SemanticLogs[aLogId] = aList;
                    aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Header), ParserUtils.GetHeader(aFac, true, this.myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Id), aFac.GetLogContainer(LogElementType.Number, aLogId, null));
                    aToParse.Message = aBuffered.Substring(aStartIndex, aNextIndex - aStartIndex);
                    ParsedSyslogMessage aParsedMessage = aParser.Parse(aToParse);

                    aList.Insert(
                        this.myLogGraph.GraphHeader.Index(aColumnDate), 
                        ParserUtils.GetLogColumn(aFac, aParsedMessage.Header.Timestamp.GetValueOrDefault().ToString(aDateFormat), aColumnDate, myLogGraph, aDateFormat, LogElementType.Date));

                    aList.Insert(
                        this.myLogGraph.GraphHeader.Index(aColumnTime),
                        ParserUtils.GetLogColumn(aFac, aParsedMessage.Header.Timestamp.GetValueOrDefault().ToString(aTimeFormat), aColumnTime, myLogGraph, aTimeFormat, LogElementType.Time));
                    
                    aList.Insert(this.myLogGraph.GraphHeader.Index(aColumnFacility), ParserUtils.GetLogColumnString(aFac, aParsedMessage.Facility.ToString(), aColumnFacility, myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(aColumnHostname), ParserUtils.GetLogColumnString(aFac, aParsedMessage.Header.HostName.ToString(), aColumnHostname, myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(aColumnAppName), ParserUtils.GetLogColumnString(aFac, aParsedMessage.Header.AppName.ToString(), aColumnAppName, myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(aColumnSeverity), ParserUtils.GetLogColumnString(aFac, aParsedMessage.Severity.ToString(), aColumnSeverity, myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(aColumnMessage), ParserUtils.GetLogColumnString(aFac, aParsedMessage.Message, aColumnMessage, myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(aColumnAdditionalData), ParserUtils.GetLogColumnString(aFac, JsonSerializer.Serialize(aParsedMessage.Data), aColumnMessage, myLogGraph));
                    if(!this.LogStore.StoreRange(aLogId, aStartIndex, aNextIndex))
                    {
                        aTracer.Error($"Unable To Store Range {aStartIndex} : {aNextIndex}");
                    }

                    aStartIndex = aNextIndex + 1;
                    aLogId++;
                    float aPercent = (float)Math.Round(aStartIndex / (float)aTotalLength * 100.0f);
                    if (aPercent >= aPreviousPercent + 10)
                    {
                        aPreviousPercent = aPercent;
                        LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, aLogId), aStartIndex, aTotalLength));
                        aLogStartId = aLogId;
                    }
                }

                HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, aLogId), aLogId, aTotalLength));
                JobFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ClearAllChunks()
        {
            myBuilder.Clear();
            myBuilder.Capacity = 0;
            this.myBuilder = new StringBuilder();
        }

        public void Reset()
        {
            this.myRawLogs = new Dictionary<int, string>();
            FreeUpMemory();
        }


        public void FreeUpMemory()
        {
            myLogGraph = new LogGraphMutable();
            LogGraph = new LogGraph(myLogGraph.GraphHeader, this.myLogGraph.SemanticLogs);
        }

        public void Dispose()
        {
            this.LogStore.Dispose();
        }
    }
}
