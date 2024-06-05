namespace Flex.LVA.Logging;

using Serilog;
using Serilog.Events;

internal class DummyLogger : ILogger
{
    public void Write(LogEvent _)
    {
        return;
    }
}