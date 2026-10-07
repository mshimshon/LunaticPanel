using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PackageDeleteReducer : IReducer<PackageManagerDiskState, PackageDeleteAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageDeleteAction action)
        => state with
        {
            IsLoading = true
        };
}
