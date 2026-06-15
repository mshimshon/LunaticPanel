using LunaticPanel.PackageManager.Application.Pulses.States.Models;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record LoadLocalPackagesDoneAction : IAction
{
    public IEnumerable<PackageLocalPulseModel> Packages { get; set; } = default!;
}
