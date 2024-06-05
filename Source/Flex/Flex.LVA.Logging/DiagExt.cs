using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.LVA.Logging
{
    public static class DiagExt
    {
        public static float RestartGetElapsedMilliSeconds(this Stopwatch theWatch)
        {
            float aRet = (float)theWatch.Elapsed.TotalMilliseconds;
            theWatch.Restart();
            return aRet;
        }
    }
}
