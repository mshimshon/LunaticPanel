using LunaticPanel.PackageManager.Application.Pulses.States.Models;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public record PackageManagerState : IStateFeatureSingleton
{
    public IEnumerable<PackageLocalPulseModel> InstalledPackages { get; init; } = Array.Empty<PackageLocalPulseModel>();
    public bool IsPackageLoading { get; init; }
    public bool IsPackageInitialized { get; init; }

}
