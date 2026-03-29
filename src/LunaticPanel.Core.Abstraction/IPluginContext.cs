namespace LunaticPanel.Core.Abstraction;

public interface IPluginContext
{
    Guid CircuitId { get; }
    bool IsMasterCircuit { get; }
    bool IsBlazorAttached { get; }
}
