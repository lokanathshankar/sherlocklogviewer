using Flex.LVA.Shared;

namespace Flex.LVA.Core
{
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Logging;
    using Flex.LVA.Shared.Containers;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class LogGraphMutable
    {
        public LogHeader GraphHeader { get; } = new LogHeader();

        public IDictionary<string, Dictionary<string, object>> Graph { get; } = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Stick with dictionary as you get free sorting as Key is always incremental
        /// </summary>
        public IDictionary<int, List<object>> SemanticLogs { get; } = new Dictionary<int, List<object>>();

        internal void Clear()
        {
            GraphHeader.Clear();
            Graph.Clear();
            SemanticLogs.Clear();
        }
    }

    internal class UniversalLogParser : StringAndFileBuilder, ILogParser, IDisposable
    {

        private LogDefinition myLogDef;
        private LogGraphMutable myLogGraph;
        private readonly LogContainerFactory myLogContainerFactory = new LogContainerFactory();
        private readonly IList<int> myFailedLines = new List<int>();


        internal UniversalLogParser(long theEngineId, IRawLogStore theRawLogStore) : base(theEngineId, theRawLogStore, typeof(UniversalLogParser))
        {
            this.Reset();
        }

        public UniversalLogParser(long theEngineId) : base(theEngineId, new RawLogStreamer(theEngineId), typeof(UniversalLogParser))
        {
            this.Reset();
        }


        public ILogGraph LogGraph { get; private set; }

        public void Parse(LogDefinition theLogDefinition, in string theStringToParse)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                Stopwatch aStopwatch = Stopwatch.StartNew();
                aTracer.Info($"Preparing Logs With LogDefinition : {theLogDefinition}");
                string aBuffered;
                if (theStringToParse == null)
                {
                    aBuffered = myBuilder.ToString();
                }
                else
                {
                    aBuffered = theStringToParse;
                }

                Task.Run(this.ClearAllChunks).HandleException(myDomain, nameof(ClearAllChunks));
                Task.Run(() => this.LogStore.LoadStore(aBuffered)).HandleException(myDomain, nameof(RawLogStreamer.LoadStore));
                myLogDef = theLogDefinition.ConvertEnglishMarkersToCSharp();
                LogSyntax aRootSyntax = DeduceHeader(myLogGraph, aBuffered);
                NestedParsing(aBuffered, myLogGraph, aRootSyntax, myLogDef);
                aTracer.Info($"Parsing Totally Took {aStopwatch.RestartGetElapsedMilliSeconds()} For Buffered String Of Length {aBuffered.Length}");
            }
        }

        private void ClearAllChunks()
        {
            myBuilder.Clear();
            myBuilder.Capacity = 0;
            this.myBuilder = new StringBuilder();
        }

        private int SkipHeader(in string theStringToParse, LogDefinition theLogDefinition)
        {
            if (theLogDefinition.HeaderLineCount == null)
            {
                return 0;
            }

            int aLinesSkipped = 0;
            int aStart = -1;
            while (aLinesSkipped < theLogDefinition.HeaderLineCount)
            {
                aStart = theStringToParse.IndexOf('\n', aStart + 1);
                aLinesSkipped++;
            }

            return ++aStart;
        }

        private void NestedParsing(in string theStringToParse, LogGraphMutable theParsedLogs, LogSyntax theRootSyntax, LogDefinition theLogDef)
        {
            int theLogId = 1;
            int aLogStartId = theLogId;
            int aStartIndex = theStringToParse.IndexOf(theRootSyntax.BeginMarker, SkipHeader(theStringToParse, theLogDef), StringComparison.Ordinal) + theRootSyntax.BeginMarker.Length;
            int aEndIndex = theStringToParse.IndexOf(theRootSyntax.EndMarker, aStartIndex, StringComparison.Ordinal);
            int aTotalLength = theStringToParse.Length;
            float aPreviousPercent = 0.0f;
            bool aRequestBreak = false;
            try
            {
                while (aEndIndex <= theStringToParse.Length)
                {
                    if (aEndIndex == -1 || aStartIndex == -1)
                    {
                        return;
                    }

                    string aLine = theStringToParse.Substring(aStartIndex, aEndIndex - aStartIndex);
                    this.LogStore.StoreRange(theLogId, aStartIndex, aEndIndex);
                    var aList = new List<object>(theParsedLogs.GraphHeader.ColumnNames.Count);
                    theParsedLogs.SemanticLogs[theLogId] = aList;
                    aList.Insert(theParsedLogs.GraphHeader.Index(Constants.Header), ParserUtils.GetHeader(myLogContainerFactory, false, theParsedLogs));
                    aList.Insert(theParsedLogs.GraphHeader.Index(Constants.Id), myLogContainerFactory.GetLogContainer(LogElementType.Number, theLogId, null));
                    if (ProcessSingleLog(theRootSyntax, ref aLine, theLogId, theParsedLogs, aList))
                    {
                        theLogId++;
                        aList[theParsedLogs.GraphHeader.Index(Constants.Header)] = ParserUtils.GetHeader(myLogContainerFactory, true, theParsedLogs);
                        aList.Insert(theParsedLogs.GraphHeader.Index(Constants.Footer), ParserUtils.GetFooter(myLogContainerFactory, string.Empty, theParsedLogs));
                    }
                    else
                    {
                        int aLogIdZeroBased = theLogId - 1;
                        if (aLogIdZeroBased != 0)
                        {
                            object aFooter = theParsedLogs.SemanticLogs[aLogIdZeroBased][theParsedLogs.GraphHeader.Index(Constants.Footer)];
                            string aAppended = aFooter + theRootSyntax.BeginMarker + aLine + theRootSyntax.EndMarker;
                            theParsedLogs.SemanticLogs[aLogIdZeroBased][theParsedLogs.GraphHeader.Index(Constants.Footer)] = myLogContainerFactory.GetLogContainer(LogElementType.String, aAppended, null);
                            theParsedLogs.SemanticLogs.Remove(theLogId);
                            if (!this.LogStore.UpdateRange(aLogIdZeroBased, aEndIndex))
                            {
                                using (var aTrace = new Tracer(myDomain))
                                {
                                    aTrace.Error($"Unable To Update Range For Id : {aLogIdZeroBased} End Index : {aEndIndex}");
                                }
                            }
                        }
                    }

                    if (aRequestBreak)
                    {
                        return;
                    }

                    aStartIndex = theStringToParse.IndexOf(theRootSyntax.BeginMarker, aEndIndex + theRootSyntax.EndMarker.Length, StringComparison.Ordinal);
                    if (aStartIndex == -1)
                    {
                        return;
                    }

                    aStartIndex += theRootSyntax.BeginMarker.Length;
                    if (aStartIndex < theStringToParse.Length)
                    {
                        aEndIndex = theStringToParse.IndexOf(theRootSyntax.EndMarker, aStartIndex + theRootSyntax.EndMarker.Length, StringComparison.Ordinal);
                    }
                    else
                    {
                        aEndIndex = theStringToParse.IndexOf(theRootSyntax.EndMarker, aStartIndex, StringComparison.Ordinal);
                    }

                    if (aEndIndex == -1)
                    {
                        aEndIndex = theStringToParse.Length;
                        aRequestBreak = true;
                    }

                    float aPercent = (float)Math.Round(aStartIndex / (float)aTotalLength * 100.0f);
                    if (aPercent >= aPreviousPercent + 10)
                    {
                        aPreviousPercent = aPercent;
                        LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, theLogId), aStartIndex, aTotalLength));
                        aLogStartId = theLogId;
                    }
                }
            }
            finally
            {
                LogChunkReady?.Invoke(this, new ProgressData(this.LogGraph, new Range(aLogStartId, theLogId), aStartIndex, aTotalLength));
                JobFinished?.Invoke(this, EventArgs.Empty);
                using (var aTrace = new Tracer(myDomain))
                {
                    string aError = $"Unable To Parse Certain Lines : '{string.Join("|", myFailedLines)}'";
                    aTrace.Error(aError);
                }
            }
        }

        private bool ProcessSingleLog(LogSyntax theRootSyntax, ref string theLogLine, int theLogId, LogGraphMutable theParsedLogs, List<object> theList)
        {
            try
            {
                if (string.IsNullOrEmpty(theLogLine))
                {
                    return false;
                }

                bool aFinalResult = true;
                int aStartIndex = 0;
                int aPrevSepLength = 0;
                if (theRootSyntax.Elements[0].EndSeparator != null)
                {
                    aPrevSepLength = theRootSyntax.Elements[0].EndSeparator.Length;
                }

                foreach (LogElement aElement in theRootSyntax.Elements)
                {
                    string aPrefix = aElement.Name;
                    aFinalResult = aFinalResult & HandleNestingParsing(aElement, ref theLogLine, theLogId, theParsedLogs, theList, ref aPrefix, ref aStartIndex, ref aPrevSepLength);
                }

                return aFinalResult;
            }
            catch (Exception aEx)
            {
#if DEBUG
                using (var aTrace = new Tracer(myDomain))
                {
                    aTrace.Error($"Unable To Parse Certain Lines : {theLogLine} {aEx}");
                }
#endif
                this.myFailedLines.Add(theLogId);
                return false;
            }
        }

        private bool HandleNestingParsing(LogElement theElement, ref string theLogLine, int theLogId, LogGraphMutable theParsedLogs, List<object> theList, ref string thePrefix, ref int theStartIndex, ref int thePrevSepLength)
        {
            if (theElement.ChildSyntaxId == null)
            {
                LogElement aCurrentElement = theElement;
                string aNextSep = theElement.EndSeparator;
                if (string.IsNullOrEmpty(aNextSep))
                {
                    theList.Add(ParserUtils.GetLogColumn(myLogContainerFactory, theLogLine.Substring(theStartIndex, theLogLine.Length - theStartIndex), aCurrentElement, theParsedLogs));
                    return true;
                }

                // Ignore Spaces
                while (theLogLine[theStartIndex] == Constants.Space)
                {
                    theStartIndex++;
                }

                int aEndIndex = theLogLine.IndexOf(aNextSep, theStartIndex + 1, StringComparison.Ordinal);
                theList.Add(ParserUtils.GetLogColumn(myLogContainerFactory, theLogLine.Substring(theStartIndex, aEndIndex - theStartIndex), aCurrentElement, theParsedLogs));
                theStartIndex = aEndIndex + aCurrentElement.EndSeparator.Length;
                thePrevSepLength = aNextSep.Length;
                return true;
            }

            LogSyntax aLogSyntax = this.FindMatchingChildSyntaxElement(theElement);
            int aPrevSepLength = 0;
            int aLocalStartIndex = 0;
            foreach (LogElement aNestedElement in aLogSyntax.Elements)
            {
                int aStartMarkerIndex = theLogLine.IndexOf(aLogSyntax.BeginMarker, theStartIndex, StringComparison.Ordinal);
                string aSubLine = null;
                if (aStartMarkerIndex != -1)
                {
                    aStartMarkerIndex += aLogSyntax.BeginMarker.Length;
                    int aEndMarkerIndex = theLogLine.IndexOf(aLogSyntax.EndMarker, aStartMarkerIndex, StringComparison.Ordinal);
                    aSubLine = theLogLine.Substring(aStartMarkerIndex, aEndMarkerIndex - aStartMarkerIndex);
                }
                else
                {
                    aSubLine = theLogLine;
                }

                if (aNestedElement.ChildSyntaxId != null)
                {
                    this.HandleNestingParsing(aNestedElement, ref aSubLine, theLogId, theParsedLogs, theList, ref thePrefix, ref aLocalStartIndex, ref aPrevSepLength);
                    continue;
                }

                if (aNestedElement.Name == null)
                {
                    //aLocalStartIndex = theLogLine.IndexOf(aNestedElement.EndSeparator, aLocalStartIndex) + aNestedElement.EndSeparator.Length;
                    continue;
                }

                theParsedLogs.Graph[aNestedElement.Name] = new Dictionary<string, object>();
                this.HandleNestingParsing(aNestedElement, ref aSubLine, theLogId, theParsedLogs, theList, ref thePrefix, ref aLocalStartIndex, ref aPrevSepLength);
            }

            return true;
        }

        private LogSyntax FindMatchingChildSyntaxElement(LogElement theElement)
        {
            return this.myLogDef.Syntaxes.FirstOrDefault(theX => theX.Id == theElement.ChildSyntaxId.Value);
        }

        private LogSyntax DeduceHeader(LogGraphMutable theParsedLogs, in string theLogs)
        {
            bool aDetectionOn = false;
            if (this.myLogDef.LogFileType == LogDataType.Auto)
            {
                aDetectionOn = true;
                this.RunLogDetection(theLogs);
            }

            theParsedLogs.GraphHeader.Add(Constants.Header, LogElementType.Bool);
            theParsedLogs.GraphHeader.Add(Constants.Id, LogElementType.Number);
            theParsedLogs.Graph[Constants.Header] = new Dictionary<string, object>();
            theParsedLogs.Graph[Constants.Id] = new Dictionary<string, object>();
            LogSyntax aRootSyntax = myLogDef.Syntaxes.First(theX => theX.SyntaxType == LogSyntaxType.Parent);
            foreach (var aElement in aRootSyntax.Elements)
            {
                string aPrefix = aElement.Name;
                HandleNestingColumnsAndFixElements(theParsedLogs, aElement, ref aPrefix);
            }

            theParsedLogs.GraphHeader.Add(Constants.Footer, LogElementType.String);
            theParsedLogs.Graph[Constants.Footer] = new Dictionary<string, object>();
            if (aDetectionOn)
            {
                this.LogGraph.GraphHeader.LogDefinition = myLogDef;
            }

            HeaderReady?.Invoke(this, this.LogGraph.GraphHeader);
            return aRootSyntax;
        }

        private void RunLogDetection(in string theLogs)
        {
            using (var aTrace = new Tracer(myDomain))
            {
                int aSampledLine = 0;
                int aStartIndex = 0;
                int aEndIndex = 0;
                IList<string> aToAnalyse = new List<string>();
                while (aSampledLine < 100)
                {
                    try
                    {
                        aEndIndex = theLogs.IndexOf('\n', aStartIndex);
                    }
                    catch(ArgumentOutOfRangeException)
                    {
                        break;
                    }

                    if (aEndIndex == -1)
                    {
                        break;
                    }

                    aToAnalyse.Add(theLogs.Substring(aStartIndex, aEndIndex - aStartIndex));
                    aStartIndex = aEndIndex + 1;
                    aSampledLine++;
                }

                new IntellisenseLogs.IntellisenseLogs().RangedDetect(aToAnalyse, out LogDefinition aTrueDef, out string aDetection);
                this.myLogDef = aTrueDef;
                this.myLogDef.AutoDetected = true;
                this.myLogDef.LogFileType = LogDataType.PlainText;
                this.myLogDef.ConvertEnglishMarkersToCSharp();
                aTrace.Info($"Detected Separator Is '{aDetection}'");
            }
        }

        private void HandleNestingColumnsAndFixElements(LogGraphMutable theParsedLogs, LogElement theElement, ref string thePrefix)
        {
            if (theElement.ChildSyntaxId == null)
            {
                theParsedLogs.GraphHeader.Add(theElement.Name, theElement.Type);
                theParsedLogs.Graph[theElement.Name] = new Dictionary<string, object>();
            }
            else
            {
                foreach (LogElement aNestedElement in this.FindMatchingChildSyntaxElement(theElement).Elements)
                {
                    if (aNestedElement.ChildSyntaxId != null)
                    {
                        HandleNestingColumnsAndFixElements(theParsedLogs, aNestedElement, ref thePrefix);
                    }

                    if (aNestedElement.Name == null)
                    {
                        continue;
                    }

                    if (thePrefix != null)
                    {
                        aNestedElement.Name = $"{thePrefix} {aNestedElement.Name}";
                    }

                    theParsedLogs.GraphHeader.Add(aNestedElement.Name, aNestedElement.Type);
                    theParsedLogs.Graph[aNestedElement.Name] = new Dictionary<string, object>();
                }
            }
        }

        public void Reset()
        {
            this.LogStore.ResetStore();
            FreeUpMemory();
        }

        public void Dispose()
        {
            this.LogStore.Dispose();
        }

        public void FreeUpMemory()
        {
            myFailedLines.Clear();
            myLogGraph = new LogGraphMutable();
            LogGraph = new LogGraph(myLogGraph.GraphHeader, this.myLogGraph.SemanticLogs);
        }

        public event EventHandler<ILogHeader> HeaderReady;

        public event EventHandler<ProgressData> LogChunkReady;

        public event EventHandler JobFinished;
    }
}
