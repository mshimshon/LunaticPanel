using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record LoadBootstrapDoneAction : IAction
{
    public List<string>? Enabled { get; init; }
    public List<string>? Disabled { get; init; }
}
