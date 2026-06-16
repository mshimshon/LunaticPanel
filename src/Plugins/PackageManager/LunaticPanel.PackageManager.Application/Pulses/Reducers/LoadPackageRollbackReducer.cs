using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadPackageRollbackReducer : IReducer<PackageManagerState, LoadPackageRollbackAction>
{
    public PackageManagerState Reduce(PackageManagerState state, LoadPackageRollbackAction action)
        => state with
        {
            IsRollingBackLoading = true
        };
}
