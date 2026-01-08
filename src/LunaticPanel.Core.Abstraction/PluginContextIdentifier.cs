namespace LunaticPanel.Core.Abstraction.Circuit;

public readonly struct PluginContextIdentifier : IEquatable<PluginContextIdentifier>
{
    public Guid CircuitId { get; }
    public string PluginId { get; }

    public PluginContextIdentifier(Guid circuitId, string pluginId)
    {
        CircuitId = circuitId;
        PluginId = pluginId;
    }

    public bool Equals(PluginContextIdentifier other)
        => CircuitId == other.CircuitId && PluginId == other.PluginId;

    public override bool Equals(object? obj)
        => obj is PluginContextIdentifier other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(CircuitId, PluginId);
}