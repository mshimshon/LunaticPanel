using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadPackageDiskFoldersDoneReducer : IReducer<PackageManagerDiskState, LoadPackageDiskFoldersDoneAction>
{
    public PackageManagerDiskState Reduce(PackageManagerDiskState state, LoadPackageDiskFoldersDoneAction action)
          => state with
          {
              AvailableRollbacks = action.AvailableRollbacks,
              Installed = action.Installed,
              PendingDelete = action.PendingDelete,
              PendingUpdates = action.PendingUpdates,
              PreInstalled = action.PreInstalled,
              RuntimePackages = action.RuntimePackages,
              IsLoading = false
          };
}
