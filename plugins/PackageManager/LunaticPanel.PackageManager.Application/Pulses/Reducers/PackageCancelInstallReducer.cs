using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PackageCancelInstallReducer : IReducer<PackageManagerDiskState, PackageCancelInstallAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, PackageCancelInstallAction action)
        => state with
        {
            IsLoading = true
        };
}
