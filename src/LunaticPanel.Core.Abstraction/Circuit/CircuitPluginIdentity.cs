namespace LunaticPanel.Core.Abstraction.Circuit;

public sealed record CircuitPluginIdentity : CircuitIdentity
{
    public IPlugin Entry { get; set; } = default!;
    public string PluginId { get; set; } = default!;
}
