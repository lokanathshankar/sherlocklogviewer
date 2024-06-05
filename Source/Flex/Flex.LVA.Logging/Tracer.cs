using System.Runtime.CompilerServices;

namespace Flex.LVA.Logging
{
    public struct Tracer : IDisposable
    {
        private readonly Domain myDomain;
        private readonly string myFunctionName;

        private ValueStopwatch myWatch;

        public Tracer(Domain theDomain, [CallerMemberName] string theCaller = "")
        {
            myDomain = theDomain;
            myFunctionName = theCaller;
            myWatch = ValueStopwatch.StartNew();
        }

        public void Debug(string theMessage)
        {
            LoggingMain.Logger.Debug(FormatMessage(ref theMessage));
        }

        public void Verbose(string theMessage)
        {
            LoggingMain.Logger.Verbose(FormatMessage(ref theMessage));
        }

        public void Warn(string theMessage)
        {
            LoggingMain.Logger.Warning(FormatMessage(ref theMessage));
        }

        public void Info(string theMessage)
        {
            LoggingMain.Logger.Information(FormatMessage(ref theMessage));
        }

        public void Error(string theMessage)
        {
            LoggingMain.Logger.Error(FormatMessage(ref theMessage));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string FormatMessage(ref string theMessage)
        {
            return string.Format("  {0,8} {1,75} - {2}",
                GetElapsedFriendly(),
                myDomain + " " +myFunctionName,
                theMessage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetElapsedFriendly()
        {
            int aMs = (int)myWatch.GetElapsedTime().TotalMilliseconds;
            if (aMs >= 1000)
            {
                return  $"{(aMs / 1000.0f):F2}S";
            }
            else
            {
                return aMs + "ms";
            }
        }

        public void Dispose()
        {
            /// NOP
        }
    }
}
