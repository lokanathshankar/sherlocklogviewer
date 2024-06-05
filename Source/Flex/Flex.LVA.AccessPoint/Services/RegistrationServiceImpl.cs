using Flex.LVA.Communication;
using Flex.LVA.Logging;
using Grpc.Core;

namespace Flex.LVA.AccessPoint.Services
{
    using Flex.LVA.Core.Interfaces;
    using System.Diagnostics;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;

    public class RegistrationServiceImpl : RegistrationService.RegistrationServiceBase
    {
        private static readonly Domain myDomain = new Domain(typeof(RegistrationServiceImpl));

        private readonly IEngineInstanceManager myEngineInstanceManager;

        public RegistrationServiceImpl(IEngineInstanceManager theEngineInstanceManager)
        {
            myEngineInstanceManager = theEngineInstanceManager;
        }
        public override Task<RegistrationData> StartServices(VoidMessage _, ServerCallContext theContext)
        {
            return new Task<RegistrationData>(() =>
            {
                RegistrationData aData = new RegistrationData
                {
                    Id = -1
                };

                using (var aTrace = new Tracer(myDomain))
                {
                    try
                    {
                        if (!myEngineInstanceManager.StartLogEngine(out long aRegistrationId, out string aAddress))
                        {
                            aTrace.Error("Unable TO Start Log Engine...");
                            return new RegistrationData();
                        }

                        aData.Id = aRegistrationId;
                        aData.RegistrationAddress = aAddress;
                        aTrace.Debug($"Engine Initialization Done For {aData.Id}");
                    }
                    catch (Exception aEx)
                    {
                        aTrace.Error($"Error Creating Channel At Error {aEx}");
                    }

                    return aData;
                }
            }).SafeStart(myDomain, nameof(StartServices));
        }

        public override Task<BoolMessage> StopServices(RegistrationData theRegistrationData, ServerCallContext theContext)
        {
            return new Task<BoolMessage>(() => new BoolMessage() { Value = this.myEngineInstanceManager.StopLogEngine(theRegistrationData.Id) }).SafeStart(myDomain, nameof(StopServices));
        }

        public override Task<StringMessage> ReadServiceVersion(VoidMessage request, ServerCallContext context)
        {
            return new Task<StringMessage>(() => {
                    StringMessage aVersionMessage=new StringMessage();
                    using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                    {
                        client.Headers.Add("User-Agent: Other");
                        aVersionMessage.Value = client.DownloadString("https://sherlocklogviewer.in/version.html");
                        return aVersionMessage;
                    }
                }).SafeStart(myDomain, nameof(ReadServiceVersion));
        }

        public override Task<VoidMessage> OpenWebPage(StringMessage request, ServerCallContext context)
        {
            return new Task<VoidMessage>(() =>
                { 
                    string url = request.Value;
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
                return new VoidMessage();
            }).SafeStart(myDomain, nameof(this.OpenWebPage));
        }
    }
}
