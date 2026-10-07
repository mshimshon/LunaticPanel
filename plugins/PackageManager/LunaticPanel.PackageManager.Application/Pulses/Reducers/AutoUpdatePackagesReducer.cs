using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class AutoUpdatePackagesReducer : IReducer<PackageManagerDiskState, AutoUpdatePackagesAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, AutoUpdatePackagesAction action)
        => state with
        {
            IsLoading = true
        };
}
