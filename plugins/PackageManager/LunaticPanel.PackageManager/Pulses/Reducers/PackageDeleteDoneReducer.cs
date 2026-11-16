using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PackageDeleteDoneReducer : IReducer<PackageManagerDiskState, PackageDeleteDoneAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageDeleteDoneAction action)
        => state with
        {
            IsLoading = false
        };
}
