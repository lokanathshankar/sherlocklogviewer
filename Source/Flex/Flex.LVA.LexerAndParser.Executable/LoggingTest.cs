using Flex.LVA.Logging;

namespace Flex.LVA.LexerAndParser.Executable
{
    internal class LoggingTest
    {
        public LoggingTest()
        {
            using (var aTrace = new Tracer(new Domain(typeof(LoggingTest))))
            {
                aTrace.Debug("Test Message");
            }
        }
    }
}
