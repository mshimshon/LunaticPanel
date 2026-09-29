using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PackageDeleteDoneReducer : IReducer<PackageManagerDiskState, PackageDeleteDoneAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageDeleteDoneAction action)
        => state with
        {
            IsLoading = false
        };
}
