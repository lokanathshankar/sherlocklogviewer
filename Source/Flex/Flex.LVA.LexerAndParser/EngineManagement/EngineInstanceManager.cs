namespace Flex.LVA.Core.EngineManagement
{
    using System.Collections.Concurrent;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Logging;

    internal class EngineResources
    {
        public IEngine LogEngine { get; init; }
        public IRenderer Renderer { get; init; }
    }

    public class EngineInstanceManager : IDisposable, IEngineInstanceManager
    {
        private Domain myDomain = new Domain(typeof(EngineInstanceManager));
        private readonly IDictionary<long, EngineResources> myEngines = new ConcurrentDictionary<long, EngineResources>();

        private long myInstanceIds;

        private readonly IRendererFactory myRendererFactory;

        public EngineInstanceManager(IRendererFactory theRendererFactory, IApplicationEvents theEventsOfApp)
        {
            myRendererFactory = theRendererFactory;
            theEventsOfApp.ApplicationShutdown += UponApplicationShutdown;
        }

        private void UponApplicationShutdown(object sender, EventArgs e)
        {
            foreach (long aEng in this.myEngines.Keys.ToArray())
            {
                this.StopLogEngine(aEng);
            }
        }

        public bool StartLogEngine(out long theRegistrationId, out string theAddress)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                //// Due to GRPC accuracy issues, if you try to get DateTime.Now.Ticks it's getting truncated to we go for time of the day.
                //// There can be conflicts with 2 of them having same time
                theRegistrationId = TimeOnly.FromDateTime(DateTime.Now).Ticks;
                while (myEngines.ContainsKey(theRegistrationId))
                {
                    theRegistrationId++;
                }

                IRenderer aRenderer = this.myRendererFactory.GetRenderer(theRegistrationId);
                IEngine aEngine = new Engine(theRegistrationId, aRenderer);
                aRenderer.OnConnectionLost += OnInstanceDisposed;
                this.myEngines[theRegistrationId] = new EngineResources() { LogEngine = aEngine, Renderer = aRenderer };
                theAddress = aRenderer.BaseAddress;
                aTracer.Info($"Engine Resource Created For {theAddress} @{theAddress}");
                return true;
            }
        }

        private void OnInstanceDisposed(object theSender, long theE)
        {
            this.StopLogEngine(theE);
        }

        public bool StopLogEngine(long theEngineId)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                if (!this.myEngines.ContainsKey(theEngineId))
                {
                    aTracer.Info($"Engine Resource Not Found For {theEngineId}");
                    return false;
                }

                this.myEngines[theEngineId].Renderer.OnConnectionLost -= OnInstanceDisposed;
                this.myEngines[theEngineId].LogEngine.Dispose();
                this.myEngines[theEngineId].Renderer.Dispose();
                aTracer.Info($"Engine Resource Destroyed For {theEngineId}");
                return this.myEngines.Remove(theEngineId);
            }
        }

        public void Dispose()
        {
            foreach (var aItem in myEngines)
            {
                aItem.Value.LogEngine.Dispose();
            }
        }

        public bool GetEngine(long theEngineId, out IEngine theEngine)
        {
            theEngine = null;
            if (!myEngines.TryGetValue(theEngineId, out EngineResources aRes))
            {
                return false;
            }

            theEngine = aRes.LogEngine;
            return true;
        }
    }
}
