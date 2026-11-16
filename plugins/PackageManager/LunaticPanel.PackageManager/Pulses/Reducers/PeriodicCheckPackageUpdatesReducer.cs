using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PeriodicCheckPackageUpdatesReducer : IReducer<PackageUpdateState, PeriodicCheckPackageUpdatesAction>
{
    public PackageUpdateState Reduce(PackageUpdateState state, PeriodicCheckPackageUpdatesAction action)
        => state with
        {
            IsLoading = true
        };
}
