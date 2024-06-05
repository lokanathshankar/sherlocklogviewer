using Flex.LVA.Shared;

namespace Flex.LVA.Core
{
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Logging;
    using Flex.LVA.Shared.Containers;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.PortableExecutable;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;
    using YamlDotNet.Core.Tokens;
    using YamlDotNet.Serialization.TypeInspectors;

    internal class XMLLogParser : ILogParser, IRawLogStore
    {
        private readonly Domain myDomain = new Domain(typeof(XMLLogParser));

        public event EventHandler<ILogHeader> HeaderReady;
        public event EventHandler<ProgressData> LogChunkReady;
        public event EventHandler JobFinished;
        private IDictionary<int, string> myRawLogs = new Dictionary<int, string>();
        private LogGraphMutable myLogGraph;
        private StringBuilder myBuilder = new StringBuilder();

        public ILogGraph LogGraph { get; private set; }

        public IRawLogStore LogStore => this;

        public Stream Writer => throw new NotImplementedException();

        public XMLLogParser(long theEngineId)
        {
            myDomain = new Domain($"{typeof(XMLLogParser).FullName}.{theEngineId}");
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

                LogContainerFactory aFac = new LogContainerFactory();
                Task.Run(this.ClearAllChunks).HandleException(myDomain, nameof(ClearAllChunks));
                this.myLogGraph.GraphHeader.Add(Constants.Header, LogElementType.Bool);
                this.myLogGraph.GraphHeader.Add(Constants.Id, LogElementType.Number);
                this.myLogGraph.Graph[Constants.Header] = new Dictionary<string, object>();
                this.myLogGraph.Graph[Constants.Id] = new Dictionary<string, object>();

                XDocument aXdoc = XDocument.Parse(aBuffered);
                IList<XElement> aElements = aXdoc.Root.Elements().ToList();
                if (aElements.Count == 0)
                {
                    aTracer.Warn("No elements found in XML Array");
                    JobFinished?.Invoke(this, EventArgs.Empty);
                }

                foreach (var aRow in aElements[0].Elements())
                {
                    this.myLogGraph.GraphHeader.Add(aRow.Name.LocalName, LogElementType.String);
                    this.myLogGraph.Graph[aRow.Name.ToString()] = new Dictionary<string, object>();
                }

                HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                int aLogId = 1;
                int aLogStartId = aLogId;
                int aTotalElements = aElements.Count;
                float aPreviousPercent = 0.0f;
                foreach (var aElement in aElements)
                {
                    List<object> aList = new List<object>(this.myLogGraph.GraphHeader.ColumnNames.Count);
                    this.myLogGraph.SemanticLogs[aLogId] = aList;
                    aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Header), ParserUtils.GetHeader(aFac, true, this.myLogGraph));
                    aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Id), aFac.GetLogContainer(LogElementType.Number, aLogId, null));
                    int aIndex = 2;
                    foreach (var aRow in aElement.Elements())
                    {
                        aList.Insert(aIndex, ParserUtils.GetLogColumnString(aFac, aRow.Value, this.myLogGraph.GraphHeader.ColumnNames[aIndex], myLogGraph));
                        aIndex++;
                    }

                    this.myRawLogs.Add(aLogId, aElement.ToString());
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
            FreeUpMemory();
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
            /// NOP
        }

        public void ResetStore()
        {
            throw new NotImplementedException();
        }
    }
}
