using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadPackageDiskFoldersReducer : IReducer<PackageManagerDiskState, LoadPackageDiskFoldersAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, LoadPackageDiskFoldersAction action)
        => state with { IsLoading = true };
}