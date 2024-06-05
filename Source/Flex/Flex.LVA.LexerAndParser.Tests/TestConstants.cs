using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.LVA.LexerAndParser.Tests
{
    internal class CategoryConstants
    {
        internal const string Basic = "Basic";
        internal const string Negative = "Negative";
        internal const string Progress = "Progress";
        internal const string Todo = "TODO";
        internal const string Time = "Time";
        private static int myEngineTestId;

    }

    internal class GeneralUtils
    {
        private static int myEngineTestId = (int)DateTime.Now.TimeOfDay.TotalSeconds;

        internal static int GetTestEngineId()
        {
            return Interlocked.Increment(ref myEngineTestId);
        }
    }
}
