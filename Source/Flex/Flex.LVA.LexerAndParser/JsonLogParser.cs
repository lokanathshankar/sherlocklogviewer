using Flex.LVA.Shared;

namespace Flex.LVA.Core
{
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Logging;
    using Flex.LVA.Shared.Containers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using YamlDotNet.Serialization.TypeInspectors;

    internal class JSONLogParser : ILogParser, IRawLogStore
    {
        private readonly Domain myDomain = new Domain(typeof(JSONLogParser));

        public event EventHandler<ILogHeader> HeaderReady;
        public event EventHandler<ProgressData> LogChunkReady;
        public event EventHandler JobFinished;
        private IDictionary<int, string> myRawLogs = new Dictionary<int, string>();
        private LogGraphMutable myLogGraph;
        private StringBuilder myBuilder = new StringBuilder();

        public ILogGraph LogGraph { get; private set; }

        public IRawLogStore LogStore => this;

        public Stream Writer => throw new NotImplementedException();

        public JSONLogParser(long theEngineId)
        {
            myDomain = new Domain($"{typeof(JSONLogParser).FullName}.{theEngineId}");
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

                Task.Run(this.ClearAllChunks).HandleException(myDomain, nameof(ClearAllChunks));
                JsonDocument aObj = JsonDocument.Parse(aBuffered);
                int aLogId = 1;
                LogContainerFactory aFac = new LogContainerFactory();
                if (aObj.RootElement.ValueKind != JsonValueKind.Array)
                {
                    aTracer.Warn("No elements found in Json Array");
                    JobFinished?.Invoke(this, EventArgs.Empty);
                }

                int aTotalElements = aObj.RootElement.GetArrayLength();
                float aPreviousPercent = 0.0f;
                int aLogStartId = aLogId;
                this.myLogGraph.GraphHeader.Add(Constants.Header, LogElementType.Bool);
                this.myLogGraph.GraphHeader.Add(Constants.Id, LogElementType.Number);
                this.myLogGraph.Graph[Constants.Header] = new Dictionary<string, object>();
                this.myLogGraph.Graph[Constants.Id] = new Dictionary<string, object>();
                foreach (JsonElement aItem in aObj.RootElement.EnumerateArray())
                {
                    int aIndex = 0;
                    List<object> aList = new List<object>(this.myLogGraph.GraphHeader.ColumnNames.Count);
                    this.myLogGraph.SemanticLogs[aLogId] = aList;
                    aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Header), ParserUtils.GetHeader(aFac, true, this.myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Id), aFac.GetLogContainer(LogElementType.Number, aLogId, null));
                    foreach (JsonProperty aProp in aItem.EnumerateObject())
                    {
                        if (!this.myLogGraph.Graph.ContainsKey(aProp.Name))
                        {
                            this.myLogGraph.Graph[aProp.Name] = new Dictionary<string, object>();
                            this.myLogGraph.GraphHeader.Add(aProp.Name, LogElementType.String);
                        }

                        aList.Insert(this.myLogGraph.GraphHeader.Index(aProp.Name), ParserUtils.GetLogColumnString(aFac, aProp.Value.ToString(), aProp.Name, myLogGraph));
                        aIndex++;
                    }

                    this.myRawLogs.Add(aLogId, aItem.GetRawText());
                    aLogId++;
                    float aPercent = (float)Math.Round(aLogId / (float)aTotalElements * 100.0f);
                    if (aPercent >= aPreviousPercent + 10)
                    {
                        aPreviousPercent = aPercent;
                        HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                        LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, aLogId), aLogId, aTotalElements));
                        aLogStartId = aLogId;
                    }
                }

                HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, aLogId), aLogId, aTotalElements));
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
            this.FreeUpMemory();
        }

        public void FreeUpMemory()
        {
            myLogGraph = new LogGraphMutable();
            LogGraph = new LogGraph(myLogGraph.GraphHeader, this.myLogGraph.SemanticLogs);
        }

        public void AppendLogChunk(IEnumerable<byte> theChunk)
        {
            foreach (byte aByte in theChunk)
            {
                myBuilder.Append((char)aByte);
            }
        }

        public void Dispose()
        {
        }

        public bool UpdateRange(int theRangeId, int theEndIndex)
        {
            return true;
        }

        public bool StoreRange(int theRangeId, int theStartIndex, int theEndIndex)
        {
            return true;
        }

        public bool GetString(int theRangeId, out string theString) => this.myRawLogs.TryGetValue(theRangeId, out theString);
        
        public void LoadStore(in string theFallbackString)
        {
        }

        public void ResetStore()
        {
        }
    }
}
