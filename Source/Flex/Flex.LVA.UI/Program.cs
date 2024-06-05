using Flex.LVA.AccessPoint;
using Flex.LVA.AccessPoint.Services;
using Flex.LVA.Core.EngineManagement;
using ElectronNET.API;
using Flex.LVA.UI;
using Flex.LVA.Shared;
using Flex.LVA.Logging;

internal class Program
{
    private static Domain myDomain = new Domain(typeof(Program));

    private static async Task Main(string[] args)
    {
        using (Tracer aTracer = new(myDomain))
        {
            if (FlexEnvironment.UsingElectron)
            {
                aTracer.Info("Running in production mode..");
                Bootstrapper.RunElectron(args);
                BrowserWindow aWindow = await Electron.WindowManager.CreateWindowAsync();
                aWindow.RemoveMenu();
                aWindow.Maximize();
                aWindow.OnClose += OnMainWindowClosed;
                Bootstrapper.Application.WaitForShutdown();
                aTracer.Info("Exiting Entire Application");
            }
            else
            {
                Bootstrapper.Run(args).Wait();
            }
        }
    }

    private static void OnMainWindowClosed()
    {
        using (Tracer aTracer = new(myDomain))
        {
            aTracer.Info("Application Shutdown Started...");
            ApplicationEvents.Instance.FireAppShutEvent();
            aTracer.Info("Application Shutdown Ends...");
        }
    }
}