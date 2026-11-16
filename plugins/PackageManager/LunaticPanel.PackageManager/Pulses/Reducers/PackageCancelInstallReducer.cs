using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PackageCancelInstallReducer : IReducer<PackageManagerDiskState, PackageCancelInstallAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageCancelInstallAction action)
        => state with
        {
            IsLoading = true
        };
}
