using Flex.LVA.AccessPoint.Services;
using Flex.LVA.Core.EngineManagement;
using Flex.LVA.Logging;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using Grpc.Core;
using Flex.LVA.Communication;
using System.Net.WebSockets;
using System.Text;

namespace Flex.LVA.UI
{
    using ElectronNET.API;

    using Flex.LVA.AccessPoint;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared;
    using Microsoft.Extensions.FileProviders;
    using System;

    public static class Bootstrapper
    {
        private static Domain myDomain = new Domain(typeof(Bootstrapper));
        public static WebApplication Application { private set; get; }

        public static Task Run(string[] theArgs)
        {
            using (var aTrace = new Tracer(myDomain))
            {
                aTrace.Info($"Bootstrapping main app With args {string.Join("|", theArgs)}");
                return new Task(
                    () =>
                        {
                            var aBuilder = WebApplication.CreateBuilder(theArgs);
                            ConfigureServices(aBuilder);
                            Application = aBuilder.Build();
                            SetupApplicationResources(Application);
                            Application.Run();
                            aTrace.Info("Bootstrapping main done");
                        },
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning).SafeStart(myDomain, nameof(Bootstrapper));
            }
        }

        public static async void RunElectron(string[] theArgs)
        {
            using (var aTrace = new Tracer(myDomain))
            {
                aTrace.Info($"Bootstrapping main electron app With args {string.Join("|", theArgs)}");
                var aBuilder = WebApplication.CreateBuilder(theArgs);
                aBuilder.WebHost.UseElectron(theArgs);
                ConfigureServices(aBuilder);
                aBuilder.Services.AddElectron();
                Application = aBuilder.Build();
                SetupApplicationResources(Application);
                aTrace.Info("Bootstrapping main done");
                await Application.StartAsync();
            }
        }

        private static void ConfigureServices(WebApplicationBuilder aBuilder)
        {
            aBuilder.Services.AddGrpc(
                theOptions =>
                {
                    theOptions.EnableDetailedErrors = true;
                    theOptions.MaxReceiveMessageSize = int.MaxValue;
                    theOptions.MaxSendMessageSize = int.MaxValue;
                });

            aBuilder.Services.Configure<KestrelServerOptions>(
                options =>
                {
                    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
                });

            aBuilder.Services.AddSingleton<IApplicationEvents>(ApplicationEvents.Instance);
            aBuilder.Services.AddSingleton<IEngineInstanceManager, EngineInstanceManager>();
            aBuilder.Services.AddSingleton<IRendererFactory, WebSocketRendererFactory>();
            aBuilder.Services.AddCors();
            aBuilder.Services.AddControllersWithViews();
        }

        private static void SetupApplicationResources(WebApplication theApplication)
        {
            theApplication.UseWebSockets(new WebSocketOptions());
            theApplication.UseRouting();
            theApplication.UseStaticFiles();
            theApplication.UseCors(
                theOptions => theOptions.SetIsOriginAllowed(theX => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            );

            theApplication.UseGrpcWeb();
            theApplication.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");

            theApplication.MapFallbackToFile("index.html");
            theApplication.MapGrpcService<EngineServiceImpl>().EnableGrpcWeb().RequireCors(theX => theX.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            theApplication.MapGrpcService<RegistrationServiceImpl>().EnableGrpcWeb().RequireCors(theX => theX.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            theApplication.MapGrpcService<LoggingServiceImpl>().EnableGrpcWeb().RequireCors(theX => theX.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        }
    }

    public class ApplicationEvents  : IApplicationEvents
    {
        public event EventHandler ApplicationShutdown;

        internal void FireAppShutEvent()
        {
            ApplicationShutdown?.Invoke(null, EventArgs.Empty);
        }

        private ApplicationEvents()
        {
        }

        public static ApplicationEvents Instance = new ApplicationEvents();
    }
}
