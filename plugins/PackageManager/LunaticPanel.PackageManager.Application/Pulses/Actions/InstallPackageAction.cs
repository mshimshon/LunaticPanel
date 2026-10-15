using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record InstallPackageAction : IAction
{
    public PackageManifestPayload Target { get; set; } = default!;
    public RepositorySourcePayload? Source { get; set; }
}
