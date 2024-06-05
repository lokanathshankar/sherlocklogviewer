namespace Flex.LVA.AccessPoint.Services;

using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using Flex.LVA.Communication;
using Flex.LVA.Core.Interfaces;
using Flex.LVA.Logging;
using Flex.LVA.Shared;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Hosting;

internal class WebSocketRenderer : IRenderer
{
    private long myId;
    private Domain myDomain;

    private readonly string myPingAliveAddress;

    private readonly CancellationTokenSource myCancellationTokenSource = new CancellationTokenSource();

    private readonly SocketLoop<string, bool> myPingAliveLoop;
    private readonly SocketLoop<LogHeader, bool> myHeaderLoop;
    private readonly SocketLoop<SymanticLogs, bool> myLogDataLoop;
    private readonly SocketLoop<bool, bool> myLogRenderLoop;
    private readonly SocketLoop<int, bool> myUpdateProgressLoop;

    private WebApplication mySubApplication;
    private Task myPingTask;

    static int FreeTcpPort()
    {
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }

    public WebSocketRenderer(long theId)
    {
        myId = theId;
        myDomain = new Domain($"{typeof(WebSocketRenderer).FullName}.{theId}");
        myPingAliveLoop = new SocketLoop<string, bool>(theId, nameof(PingAlive));
        myHeaderLoop = new SocketLoop<LogHeader, bool>(theId, nameof(NegotiateHeader));
        myLogDataLoop = new SocketLoop<SymanticLogs, bool>(theId, nameof(NegotiateData));
        myLogRenderLoop = new SocketLoop<bool, bool>(theId, nameof(RenderLogs));
        myUpdateProgressLoop = new SocketLoop<int, bool>(theId, nameof(UpdateProgress));
        using (var aTracer = new Tracer(myDomain))
        {
            string aHttpAddress = $"http://localhost:{FreeTcpPort()}";
            var aBuilder = WebApplication.CreateBuilder();
            aBuilder.WebHost.UseUrls(aHttpAddress);
            this.BaseAddress = $"{aHttpAddress.Replace("http", "ws")}/{theId}";
            mySubApplication = aBuilder.Build();
            mySubApplication.UseWebSockets();
            HashSet<Tuple<string, ISocketLoop>> mySocketLoops = new HashSet<Tuple<string, ISocketLoop>>() {
                Tuple.Create(nameof(RenderLogs), (ISocketLoop)myLogRenderLoop),
                Tuple.Create(nameof(PingAlive), (ISocketLoop)myPingAliveLoop),
                Tuple.Create(nameof(NegotiateData), (ISocketLoop)myLogDataLoop),
                Tuple.Create(nameof(NegotiateHeader), (ISocketLoop)myHeaderLoop),
                Tuple.Create(nameof(UpdateProgress), (ISocketLoop)myUpdateProgressLoop)};
            foreach (var aItem in mySocketLoops)
            {
                string aEndPoint = $"/{theId}/{aItem.Item1}".ToLower();
                MapEndPoints(aEndPoint, aItem.Item2);
            }

            myPingTask = new Task(this.PingAlive).SafeStart(this.myDomain, "PingAlive");
            mySubApplication.RunAsync(myCancellationTokenSource.Token);
        }
    }

    private void MapEndPoints(string theEndPointName, ISocketLoop theSocketLoop)
    {
        using (var aTracer = new Tracer(myDomain))
        {
            aTracer.Info($"Opening Channel With {theEndPointName} With Base {BaseAddress}");
            mySubApplication.Map(
                        theEndPointName,
                        async theContext =>
                        {
                            aTracer.Debug($"Request Incoming @{theEndPointName}");
                            if (theContext.WebSockets.IsWebSocketRequest)
                            {
                                await theSocketLoop.RunLoop(theContext);
                            }

                            theContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        });
        }
    }

    public void Dispose()
    {
        this.myPingTask.Wait();
        this.myPingTask.Dispose();
        this.myCancellationTokenSource.Cancel();
        this.mySubApplication.DisposeAsync().AsTask().Wait();
        this.myPingAliveLoop.Dispose();
        this.myHeaderLoop.Dispose();
        this.myLogDataLoop.Dispose();
        this.myLogRenderLoop.Dispose();
        this.myUpdateProgressLoop.Dispose();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool NegotiateHeader(ILogHeader theLogHeader)
    {
        using var aTracer = new Tracer(this.myDomain);
        {
            if (!this.myHeaderLoop.Connected)
            {
                aTracer.Debug("Connection Yet To Be Done...");
                return false;
            }

            this.myHeaderLoop.Request = LogHeader.CreateLogHeader(theLogHeader);
            this.myHeaderLoop.SitAndWatch();
            return this.myHeaderLoop.Response;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool NegotiateData(List<List<object>> theReadOnlyDictionary, out IList<int> theFailedLogs)
    {
        theFailedLogs = null;
        using var aTracer = new Tracer(this.myDomain);
        {
            if (!this.myLogDataLoop.Connected)
            {
                aTracer.Debug("Connection Yet To Be Done...");
                return false;
            }

            this.myLogDataLoop.Request = new SymanticLogs() { Logs = theReadOnlyDictionary };
            this.myLogDataLoop.SitAndWatch();
            return this.myLogDataLoop.Response;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool UpdateProgress(int thePercent)
    {
        using var aTracer = new Tracer(this.myDomain);
        {
            var aLoop = myUpdateProgressLoop;
            if (!aLoop.Connected)
            {
                aTracer.Debug("Connection Yet To Be Done...");
                return false;
            }

            aLoop.Request = thePercent;
            aLoop.SitAndWatch();
            return aLoop.Response;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool RenderLogs()
    {
        using var aTracer = new Tracer(this.myDomain);
        {
            if (!this.myLogRenderLoop.Connected)
            {
                aTracer.Debug("Connection Yet To Be Done...");
                return false;
            }

            this.myLogRenderLoop.Request = true;
            this.myLogRenderLoop.SitAndWatch();
            return this.myLogRenderLoop.Response;
        }
    }

    public event EventHandler<long>? OnConnectionLost;

    public string BaseAddress { get; }

    private void PingAlive()
    {
        using var aTracer = new Tracer(this.myDomain);
        {
            while (true)
            {
                // TODO : This really is not needed, because socket will automatically abort, we keep this for now to test if connection itself are fine
                Thread.Sleep(5000);
                if (!this.myPingAliveLoop.Connected)
                {
                    aTracer.Verbose("Connection Yet To Be Done, Waiting...");
                    continue;
                }

                this.myPingAliveLoop.Request = "Ping Check";
                this.myPingAliveLoop.SitAndWatch();
                if (this.myPingAliveLoop.Response)
                {
                    aTracer.Verbose($"Ping Check Ok, All Good Response : {this.myPingAliveLoop.Response}");
                    continue;
                }

                aTracer.Warn("Ping Check Not Ok...");
                this.OnConnectionLost?.Invoke(this, myId);
                return;
            }
        }
    }

}