using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record PeriodicCheckPackageUpdatesDoneAction : IAction
{
    public ICollection<PackageManifestPayload> AvailableUpdates { get; set; } = Array.Empty<PackageManifestPayload>();
}
