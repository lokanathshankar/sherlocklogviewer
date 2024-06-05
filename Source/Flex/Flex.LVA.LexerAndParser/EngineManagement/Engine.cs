namespace Flex.LVA.Core.EngineManagement
{
    using System.Diagnostics;

    using Flex.LVA.Logging;
    using Flex.LVA.Shared;

    using System.Runtime.Serialization;
    using System.Timers;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared.Containers;
    using System.Text;
    using System.Buffers;
    using System.Runtime.InteropServices;

    public class Engine : IDisposable, IEngine
    {
        private readonly Domain myDomain;
        private readonly long myId;

        private IRenderer myRenderer;

        private bool myDisposed;
        private bool myHeaderNegoFailed;

        public event EventHandler<long> InstanceDisposedRequest;

        internal IDictionary<LogDataType, ILogParser> Parsers = new Dictionary<LogDataType, ILogParser>();
        private LogDefinition myLogDefinition;
        private ILogParser myCurrentParser;

        internal Engine(ILogParser theLogParser, long theEngineId, IRenderer theRenderer)
        {
            myDomain = new Domain($"{typeof(Engine).FullName}.{theEngineId}");
            using (var aTracer = new Tracer(myDomain))
            {
                this.myId = theEngineId;
                this.myRenderer = theRenderer;
                this.Parsers[LogDataType.PlainText] = theLogParser;
                this.myCurrentParser = theLogParser;
                SubscribeEvents();
                aTracer.Debug("Engine Construction Done!");
            }
        }

        public Engine(long theEngineId, IRenderer theRenderer)
        {
            myDomain = new Domain($"{typeof(Engine).FullName}.{theEngineId}");
            using (var aTracer = new Tracer(myDomain))
            {
                this.myId = theEngineId;
                this.myRenderer = theRenderer;
                this.Parsers[LogDataType.PlainText] = new UniversalLogParser(theEngineId);
                this.Parsers[LogDataType.Json] = new JSONLogParser(theEngineId);
                this.Parsers[LogDataType.Xml] = new XMLLogParser(theEngineId);
                this.Parsers[LogDataType.Evtx] = new EventLogParser(theEngineId);
                this.Parsers[LogDataType.Delimited] = new DelimitedLogParser(theEngineId);
                this.Parsers[LogDataType.Auto] = new UniversalLogParser(long.MaxValue - theEngineId);
                SubscribeEvents();
                aTracer.Debug("Engine Construction Done!");
            }
        }

        private void SubscribeEvents()
        {
            foreach (KeyValuePair<LogDataType, ILogParser> aParser in Parsers)
            {
                aParser.Value.HeaderReady += OnHeaderReady;
                aParser.Value.LogChunkReady += OnChunkReady;
                aParser.Value.JobFinished += OnJobFinished;
            }
        }

        private void OnJobFinished(object sender, EventArgs e)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                ValueStopwatch aStopwatch = ValueStopwatch.StartNew();
                try
                {
                    if (!myRenderer.RenderLogs())
                    {
                        aTracer.Error($"Log Rendering Failed.");
                        return;
                    }
                }
                finally
                {
                    this.myCurrentParser.FreeUpMemory();
                }

                aTracer.Info($"Logs Rendered With A Time Of {aStopwatch.RestartGetElapsedMilliSeconds()} With Total Lines Of {myCurrentParser.LogGraph.SemanticLogs.Count}...");
            }
        }

        private void OnChunkReady(object sender, ProgressData e)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                ValueStopwatch aStopwatch = ValueStopwatch.StartNew();
                if (!myHeaderNegoFailed)
                {
                    aTracer.Error($"Log header negotiation failed before.");
                    throw new InvalidDataContractException($"Log header negotiation failed for Engine Id : {myId}");
                    return;
                }

                int aRange = e.LineRange.End.Value - e.LineRange.Start.Value;
                List<List<object>> aLogsToSend = new List<List<object>>(aRange);
                foreach (var aLogRow in myCurrentParser.LogGraph.SemanticLogs.Skip(e.LineRange.Start.Value - 1).Take(aRange))
                {
                    aLogsToSend.Add(aLogRow.Value.Select(theX => ((LogContainer)theX).Value).ToList());
                }

                if (!myRenderer.NegotiateData(
                                aLogsToSend,
                                out IList<int> aFailedLogs))
                {
                    aTracer.Error($"Log rendering failed Failed Log IDs : {string.Join(",", aFailedLogs)}");
                }

                this.myRenderer.UpdateProgress((int)e.ProgressPercentage);
                aTracer.Info($"Negotiation Of Data Took {aStopwatch.RestartGetElapsedMilliSeconds()} With Total Lines Of {myCurrentParser.LogGraph.SemanticLogs.Count} And Current Progress Of {e.ProgressPercentage}");
            }
        }

        private void OnHeaderReady(object sender, ILogHeader e)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                ValueStopwatch aStopwatch = ValueStopwatch.StartNew();
                this.myHeaderNegoFailed = myRenderer.NegotiateHeader(myCurrentParser.LogGraph.GraphHeader);
                if (!this.myHeaderNegoFailed)
                {
                    aTracer.Error($"Log header negotiation failed.");
                    throw new InvalidDataContractException($"Log header negotiation failed for Engine Id : {myId}");
                    return;
                }

                aTracer.Info($"Negotiation Of Header Took {aStopwatch.RestartGetElapsedMilliSeconds()}.");
            }
        }

        public void Dispose()
        {
            using (var aTracer = new Tracer(myDomain))
            {
                if (myDisposed)
                {
                    return;
                }

                this.myDisposed = true;
                foreach (KeyValuePair<LogDataType, ILogParser> aParser in Parsers)
                {
                    aParser.Value.Reset();
                    aParser.Value.HeaderReady -= OnHeaderReady;
                    aParser.Value.LogChunkReady -= OnChunkReady;
                    aParser.Value.JobFinished -= OnJobFinished;
                    aParser.Value.Dispose();
                }

                aTracer.Info("Instance Disposed.");
            }
        }

        public void AppendLogChunk(IEnumerable<byte> theChunk)
        {
            if (myDisposed)
            {
                throw new ObjectDisposedException($"Engine Instance With ID {myId} already disposed");
            }

            this.myCurrentParser.AppendLogChunk(theChunk);
        }

        public void PrepareLog()
        {
            if (myDisposed)
            {
                throw new ObjectDisposedException($"Engine Instance With ID {myId} already disposed");
            }

            ValueStopwatch aStopwatch = new ValueStopwatch();
            using (var aTracer = new Tracer(myDomain))
            {
                aStopwatch.RestartGetElapsedMilliSeconds();
                this.myCurrentParser.Parse(myLogDefinition, null);
                aTracer.Info($"Parsing Totally Took {aStopwatch.RestartGetElapsedMilliSeconds()}");
            }
        }

        public bool GetRawLog(int theLogId, out string theRawLog)
        {
            return this.myCurrentParser.LogStore.GetString(theLogId, out theRawLog);
        }


        public bool GetRawLogs(IList<int> theLogIds, out string theRawLogs)
        {
            StringBuilder aBuilder = new StringBuilder();
            foreach (int aId in theLogIds)
            {
                if (this.myCurrentParser.LogStore.GetString(aId, out string aToAppend))
                {
                    // TODO : See Test RawLosgStreamerMustPreserverSeperators
                    aBuilder.AppendLine(aToAppend);
                }
            }

            theRawLogs = aBuilder.ToString();
            return true;
        }

        public void PrepareResources(string theLogDefinition)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                ValueStopwatch aStopwatch = new ValueStopwatch();
                aTracer.Info($"PrepareResources With LogDefinition : {theLogDefinition}");
                this.myLogDefinition = SerilizationUtils.DeSerilizeFromJson<LogDefinition>(theLogDefinition);
                ILogParser myPrevParser = myCurrentParser;
                this.myCurrentParser = this.Parsers[myLogDefinition.LogFileType];
                this.myLogDefinition.ConvertEnglishMarkersToCSharp();
                if(myPrevParser != myCurrentParser)
                {
                    myPrevParser?.Reset();
                }
                else
                {
                    myCurrentParser.Reset();
                }

                aTracer.Info($"PrepareResources Totally Took {aStopwatch.RestartGetElapsedMilliSeconds()}");
            }
        }
    }
}
