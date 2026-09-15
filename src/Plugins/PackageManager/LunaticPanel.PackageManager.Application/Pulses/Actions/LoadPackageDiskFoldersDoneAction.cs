using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record LoadPackageDiskFoldersDoneAction : IAction
{
    public IEnumerable<PackagePayload> PendingUpdates { get; set; } = Array.Empty<PackagePayload>();
    public IEnumerable<PackagePayload> AvailableRollbacks { get; set; } = Array.Empty<PackagePayload>();
    public IEnumerable<PackagePayload> PendingDelete { get; set; } = Array.Empty<PackagePayload>();
    public IEnumerable<PackagePayload> Installed { get; set; } = Array.Empty<PackagePayload>();
}
