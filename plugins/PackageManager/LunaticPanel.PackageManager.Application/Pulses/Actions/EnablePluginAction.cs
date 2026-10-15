using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record EnablePluginAction : IAction
{
    public string Id { get; set; } = default!;

}
