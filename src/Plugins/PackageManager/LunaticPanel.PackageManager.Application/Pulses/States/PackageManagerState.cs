using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public record PackageManagerState : IStateFeatureSingleton
{
    public IEnumerable<PackagePayload> Installed { get; init; } = Array.Empty<PackagePayload>();
    public bool IsInstalledPackageLoading { get; init; }

    public IEnumerable<PackagePayload> Enabled { get; init; } = Array.Empty<PackagePayload>();
    public bool EnabledPackageLoading { get; init; }

    public IEnumerable<PackagePayload> Disabled { get; init; } = Array.Empty<PackagePayload>();
    public bool DisabledPackagesLoading { get; init; }



}
