using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PackageCancelInstallDoneReducer : IReducer<PackageManagerDiskState, PackageCancelInstallDoneAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageCancelInstallDoneAction action)
        => state with
        {
            IsLoading = false
        };
}
