using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class AutoUpdatePackagesDoneReducer : IReducer<PackageManagerDiskState, AutoUpdatePackagesDoneAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, AutoUpdatePackagesDoneAction action)
        => state with
        {
            IsLoading = false

        };
}
