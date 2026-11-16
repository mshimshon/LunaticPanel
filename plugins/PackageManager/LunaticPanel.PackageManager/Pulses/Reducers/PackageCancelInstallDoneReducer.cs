using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PackageCancelInstallDoneReducer : IReducer<PackageManagerDiskState, PackageCancelInstallDoneAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageCancelInstallDoneAction action)
        => state with
        {
            IsLoading = false
        };
}
