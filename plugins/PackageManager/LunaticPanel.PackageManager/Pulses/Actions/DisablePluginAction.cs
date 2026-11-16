using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Actions;

public sealed record DisablePluginAction : IAction
{
    public string Id { get; set; } = default!;
}
