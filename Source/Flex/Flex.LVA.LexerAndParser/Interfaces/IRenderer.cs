using Flex.LVA.Shared;

namespace Flex.LVA.Core.Interfaces
{
    public interface IRenderer : IDisposable
    {
        bool NegotiateHeader(ILogHeader theColumnsAndTypes);

        bool NegotiateData(List<List<object>> theReadOnlyDictionary, out IList<int> theFailedLogs);

        bool RenderLogs();

        bool UpdateProgress(int thePercent);
        
        event EventHandler<long> OnConnectionLost;

        string BaseAddress { get; }
    }
}
