using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadLocalPackagesDoneReducer : IReducer<PackageManagerState, LoadLocalPackagesDoneAction>
{
    public PackageManagerState Reduce(PackageManagerState state, LoadLocalPackagesDoneAction action)
        => state with
        {
            IsPackageLoading = false,
            IsPackageInitialized = true,
            InstalledPackages = action.Packages
        };
}
