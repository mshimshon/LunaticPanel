using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class LoadPackageDiskFoldersReducer : IReducer<PackageManagerDiskState, LoadPackageDiskFoldersAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, LoadPackageDiskFoldersAction action)
        => state with { IsLoading = true };
}
