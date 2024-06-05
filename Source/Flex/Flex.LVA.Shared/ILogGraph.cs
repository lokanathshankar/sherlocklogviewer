namespace Flex.LVA.Shared;

public interface ILogGraph
{
    public ILogHeader GraphHeader { get; }

    /// <summary>
    /// Stick with dictionary as you get free sorting as Key is always incremental
    /// </summary>
    public IReadOnlyDictionary<int, List<object>> SemanticLogs { get; }

    public IReadOnlyDictionary<int, List<object>> FilteredSemanticLogs { get; }
}