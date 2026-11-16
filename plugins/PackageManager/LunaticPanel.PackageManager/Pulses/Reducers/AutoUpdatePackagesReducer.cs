using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class AutoUpdatePackagesReducer : IReducer<PackageManagerDiskState, AutoUpdatePackagesAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, AutoUpdatePackagesAction action)
        => state with
        {
            IsLoading = true
        };
}
