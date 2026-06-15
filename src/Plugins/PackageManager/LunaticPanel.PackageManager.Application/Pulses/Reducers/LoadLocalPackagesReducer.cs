using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadLocalPackagesReducer : IReducer<PackageManagerState, LoadLocalPackagesAction>
{
    public PackageManagerState Reduce(PackageManagerState state, LoadLocalPackagesAction action)
        => state with
        {
            IsInstalledPackageLoading = true
        };
}
