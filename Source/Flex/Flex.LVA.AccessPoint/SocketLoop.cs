
namespace Flex.LVA.AccessPoint;

using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Flex.LVA.Logging;
using Flex.LVA.Shared;
using Microsoft.AspNetCore.Http;
using Flex.LVA.Logging;
using System.Text.Json;

internal interface ISocketLoop
{
    internal bool Connected { get; }
    internal Task RunLoop(HttpContext theContext);
}
internal class SocketLoop<TRequest, TResponse> : ISocketLoop, IDisposable
{
    private long myPreviousAllocSize;
    
    private Domain myDomain;

    public TRequest Request { get; set; } = default;

    public TResponse Response { get; set; } = default;

    public bool Connected { get; set; }

    private AutoResetEvent mySyncLoopEvent = new AutoResetEvent(false);
    private Task myTask;
    private string myChannelShortName;
    private readonly CancellationTokenSource myCalncel = new CancellationTokenSource();
    public SocketLoop(long theId, string theChannel)
    {
        myDomain = new Domain($"Flex.LVA.AccessPoint.SocketLoop.{theId}.{theChannel}");
        using (var aTracer = new Tracer(myDomain))
        {
            aTracer.Info($"SocketLoop Constructed For Use...");
        }
    }

    public Task RunLoop(HttpContext theContext)
    {
        myTask = new Task(() =>
        {
            using (var aTracer = new Tracer(myDomain))
            using (var aWebSocket = theContext.WebSockets.AcceptWebSocketAsync().Result)
            {
                try
                {
                    aTracer.Info($"SocketLoop Connected and Running...");
                    Connected = true;
                    byte[] aSourceArray = new byte[10 * 1024 * 1024];
                    Stopwatch aWatch = Stopwatch.StartNew();
                    while (!myCalncel.IsCancellationRequested)
                    {
                        aWatch.Restart();
                        mySyncLoopEvent.WaitOne();
                        if(myCalncel.IsCancellationRequested)
                        {
                            return;
                        }

                        aTracer.Verbose($"Sending Request Message {myChannelShortName}");
                        aWatch.Restart();
                        long aBytesToSend = 0;
                        float aEncodeTime = 0;
                        using (MemoryStream aStream = new MemoryStream((int)myPreviousAllocSize))
                        {
                            SerilizationUtils.SerilizeToJsonBytes(aStream, Request);
                            aEncodeTime = aWatch.RestartGetElapsedMilliSeconds();
                            aBytesToSend = aStream.Length;
                            aStream.Seek(0, SeekOrigin.Begin);
                            bool aStreamEnd = false;
                            while (!aStreamEnd)
                            {
                                int aReadCount = aStream.Read(aSourceArray, 0, aSourceArray.Length);
                                aStreamEnd = aReadCount != aSourceArray.Length;
                                aWebSocket.SendAsync(new ArraySegment<byte>(aSourceArray, 0, aReadCount), WebSocketMessageType.Text, aStreamEnd, myCalncel.Token).Wait();
                            }

                            myPreviousAllocSize = aStream.Length;
                        }

                        if (aBytesToSend > 1 * 1024 * 1024)
                        {
                            aTracer.Info($"Sending {GetMbValue(aBytesToSend)} MB of Data Type {typeof(TRequest)} With Encoding Time Of {aEncodeTime:F2} and Transfer Time Of {aWatch.RestartGetElapsedMilliSeconds():F2}");
                        }

                        aTracer.Verbose($"Reading Response Message...");
                        WebSocketReceiveResult aResult = aWebSocket.ReceiveAsync(aSourceArray, myCalncel.Token).Result;
                        this.Response = SerilizationUtils.DeSerilizeFromJson<TResponse>(Encoding.UTF8.GetString(aSourceArray, 0, aResult.Count));
                        mySyncLoopEvent.Set();
                    }
                }
                catch (Exception aEx)
                {
                    aTracer.Error($"Unexpected failure in socket loop {aEx}");
                    this.Response = default;
                    return;
                }
                finally
                {
                    mySyncLoopEvent.Set();
                }
            }
        }, TaskCreationOptions.LongRunning).SafeStart(myDomain, "RunLoop");
        return myTask;
    }

    private float GetMbValue(long theBytes)
    {
        return theBytes / (1024.0f * 1024.0f);
    }
    internal void SitAndWatch()
    {
        this.Response = default;
        mySyncLoopEvent.Set();
        mySyncLoopEvent.WaitOne();
    }

    public void Dispose()
    {
        this.myCalncel.Cancel();
        this.mySyncLoopEvent.Set();
        this.myTask.Wait();
    }
}
