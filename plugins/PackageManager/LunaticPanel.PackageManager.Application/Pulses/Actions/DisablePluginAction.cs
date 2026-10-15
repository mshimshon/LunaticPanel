using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record DisablePluginAction : IAction
{
    public string Id { get; set; } = default!;
}
