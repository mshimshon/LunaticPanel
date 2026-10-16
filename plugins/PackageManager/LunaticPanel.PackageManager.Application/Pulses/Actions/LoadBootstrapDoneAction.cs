using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record LoadBootstrapDoneAction : IAction
{
    public List<string>? Enabled { get; set; }
    public List<string>? Disabled { get; set; }
}
