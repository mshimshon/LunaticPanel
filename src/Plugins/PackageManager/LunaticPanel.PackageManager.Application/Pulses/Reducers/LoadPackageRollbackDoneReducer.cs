using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadPackageRollbackDoneReducer : IReducer<PackageManagerState, LoadPackageRollbackDoneAction>
{
    public PackageManagerState Reduce(PackageManagerState state, LoadPackageRollbackDoneAction action)
       => state with
       {
           IsRollingBackLoading = false,
           AvailableRollbackPackages = action.UpdateRollback != default ? action.UpdateRollback : state.AvailableRollbackPackages
       };
}
