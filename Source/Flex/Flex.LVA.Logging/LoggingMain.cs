using Serilog;

namespace Flex.LVA.Logging
{
    public static class FlexDirectories
    {
        public static string LvaDiagDir => Path.Combine(Path.GetTempPath(), "flexlva");
        public static string LvaTempDir => Path.Combine(FlexDirectories.LvaDiagDir, "temp");
        public static string LvaLogDir => Path.Combine(FlexDirectories.LvaDiagDir, "logs");
        static FlexDirectories()
        {
            Directory.CreateDirectory(LvaDiagDir);
            Directory.CreateDirectory(LvaTempDir);
            Directory.CreateDirectory(LvaLogDir);
        }
    }

    internal static class LoggingMain
    {
        public static ILogger Logger { get; }

        static LoggingMain()
        {
            try
            {
                string aFilePath = Path.Combine(FlexDirectories.LvaLogDir, "FlexLVALogs_.log");
                string aOutputTemplate = "{ProcessId,6} {ThreadId,5} {Timestamp:HH:mm:ss.fff} {Level:u3} {Function}{Message}{NewLine}{Exception}";
                Logger = Log.Logger = new LoggerConfiguration()
                             .Enrich.WithProcessName()
                             .Enrich.WithThreadId()
                             .Enrich.WithProcessId()
                             .MinimumLevel.Debug()
                             .WriteTo.Console(outputTemplate: aOutputTemplate)
                             .WriteTo.File(aFilePath, rollingInterval: RollingInterval.Day, outputTemplate: aOutputTemplate)
                             .CreateLogger();
                using (var aTracer = new Tracer(new Domain(typeof(LoggingMain))))
                {
                    aTracer.Info("=========================================================================================");
                    aTracer.Info("=====================================Session Start=======================================");
                    aTracer.Info("=========================================================================================");
                }
            }
            catch (Exception)
            {
                Logger = new DummyLogger();
            }
        }
    }
}
