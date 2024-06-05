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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;

    using System.Numerics;
    using CsvHelper;
    using CsvHelper.Configuration;

    internal class DelimitedLogParser : ILogParser, IRawLogStore
    {
        private readonly Domain myDomain = new Domain(typeof(DelimitedLogParser));

        public event EventHandler<ILogHeader> HeaderReady;
        public event EventHandler<ProgressData> LogChunkReady;
        public event EventHandler JobFinished;
        private IDictionary<int, string> myRawLogs = new Dictionary<int, string>();
        private LogGraphMutable myLogGraph;
        private StringBuilder myBuilder = new StringBuilder();

        private CsvConfiguration myOptions;

        public ILogGraph LogGraph { get; private set; }

        public IRawLogStore LogStore => this;

        public Stream Writer => throw new NotImplementedException();

        public DelimitedLogParser(long theEngineId)
        {
            myDomain = new Domain($"{typeof(DelimitedLogParser).FullName}.{theEngineId}");
            myOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
                                {
                                    DetectDelimiter = true,
                                    BadDataFound = null,
                                    MissingFieldFound = null,
            };
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
                using (var aReader = new CsvReader(new StringReader(aBuffered), this.myOptions))
                {
                    int aTotalElements = int.MaxValue;
                    int aLogId = 1;
                    LogContainerFactory aFac = new LogContainerFactory();
                    int aLogStartId = aLogId;
                    this.myLogGraph.GraphHeader.Add(Constants.Header, LogElementType.Bool);
                    this.myLogGraph.GraphHeader.Add(Constants.Id, LogElementType.Number);
                    this.myLogGraph.Graph[Constants.Header] = new Dictionary<string, object>();
                    this.myLogGraph.Graph[Constants.Id] = new Dictionary<string, object>();
                    List<string> aCsvCols = new List<string>();
                    if (!aReader.Read() ||!aReader.ReadHeader())
                    {
                        // TODO : Error handling
                        JobFinished?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    foreach (var aColumns in aReader.HeaderRecord)
                    {
                        this.myLogGraph.GraphHeader.Add(aColumns, LogElementType.String);
                        this.myLogGraph.Graph[aColumns] = new Dictionary<string, object>();
                        aCsvCols.Add(aColumns);
                    }

                    while (aReader.Read())
                    {
                        List<object> aList = new List<object>(this.myLogGraph.GraphHeader.ColumnNames.Count);
                        this.myLogGraph.SemanticLogs[aLogId] = aList;
                        aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Header), ParserUtils.GetHeader(aFac, true, this.myLogGraph));
                        aList.Insert(this.myLogGraph.GraphHeader.Index(Constants.Id), aFac.GetLogContainer(LogElementType.Number, aLogId, null));

                        foreach (string aColName in aCsvCols)
                        {
                            string aValue = aReader.GetField(aReader.GetFieldIndex(aColName));
                            if (aValue == null)
                            {
                                aList.Insert(this.myLogGraph.GraphHeader.Index(aColName), ParserUtils.GetLogColumnString(aFac, "Invalid Data", aColName, myLogGraph));
                            }
                            else
                            {
                                aList.Insert(this.myLogGraph.GraphHeader.Index(aColName), ParserUtils.GetLogColumnString(aFac, aValue, aColName, myLogGraph));
                            }
                        }

                        this.myRawLogs.Add(aLogId, aReader.Parser.Context.Parser.RawRecord);
                        aLogId++;
                        aTotalElements++;
                    }


                    HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
                    LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, aLogId), aLogId, aTotalElements));
                    JobFinished?.Invoke(this, EventArgs.Empty);
                }
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
