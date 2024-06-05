using Flex.LVA.Shared.Containers;
using System.Text.Json.Serialization;

namespace Flex.LVA.Shared
{
    public class LogGraph : ILogGraph
    {
        public LogGraph(ILogHeader theGraphHeader, IDictionary<int, List<object>> theSemanticLogs)
        {
            this.GraphHeader = theGraphHeader;
            this.SemanticLogs = theSemanticLogs.AsReadOnly();
        }

        public ILogHeader GraphHeader { get; }

        public IReadOnlyDictionary<int, List<object>> SemanticLogs { get; }

        public IReadOnlyDictionary<int, List<object>> FilteredSemanticLogs { get; }
    }
}
