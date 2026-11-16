using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PackageDeleteReducer : IReducer<PackageManagerDiskState, PackageDeleteAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageDeleteAction action)
        => state with
        {
            IsLoading = true
        };
}
