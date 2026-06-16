using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public record PackageManagerState : IStateFeatureSingleton
{
    public IEnumerable<PackagePayload> InstalledPackages { get; init; } = Array.Empty<PackagePayload>();
    public bool IsPackageLoading { get; init; }
    public IEnumerable<PackagePayload> AvailableUpdatePackages { get; init; } = Array.Empty<PackagePayload>();
    public bool IsUpdateLoading { get; init; }
    public IEnumerable<PackagePayload> AvailableRollbackPackages { get; init; } = Array.Empty<PackagePayload>();
    public bool IsRollingBackLoading { get; init; }
    public bool IsPackageInitialized { get; init; }

}
