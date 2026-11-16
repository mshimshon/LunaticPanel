using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Actions;

public sealed record EnablePluginAction : IAction
{
    public string Id { get; set; } = default!;

}
