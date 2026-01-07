namespace LunaticPanel.Core.Abstraction.Circuit;

public interface ICircuitRegistry
{
    IReadOnlyCollection<CircuitIdentity> GetActiveCircuits();
    IPluginContextService GetPluginContext(string pluginId, Guid circuitId);
    CircuitIdentity CurrentCircuit { get; }
    IReadOnlyDictionary<PluginContextIdentifier, IPluginContextService> GetPluginContexts();
    IReadOnlyDictionary<PluginContextIdentifier, CircuitPluginIdentity> GetCircuitPluginIdentities();
}
