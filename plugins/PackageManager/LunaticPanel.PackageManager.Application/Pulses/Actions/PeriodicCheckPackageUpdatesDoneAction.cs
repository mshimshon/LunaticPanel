using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record PeriodicCheckPackageUpdatesDoneAction : IAction
{
    public ICollection<PackagePayload> AvailableUpdates { get; set; } = Array.Empty<PackagePayload>();
}
