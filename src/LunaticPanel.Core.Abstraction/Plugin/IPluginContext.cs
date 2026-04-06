namespace LunaticPanel.Core.Abstraction.Plugin;

public interface IPluginContext
{
    Guid CircuitId { get; }
    bool IsMasterCircuit { get; }
    bool IsBlazorAttached { get; }
}
