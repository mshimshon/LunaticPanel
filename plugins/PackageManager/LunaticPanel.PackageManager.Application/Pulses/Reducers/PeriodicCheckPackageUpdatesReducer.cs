using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PeriodicCheckPackageUpdatesReducer : IReducer<PackageUpdateState, PeriodicCheckPackageUpdatesAction>
{
    public PackageUpdateState Reduce(PackageUpdateState state, PeriodicCheckPackageUpdatesAction action)
        => state with
        {
            IsLoading = true
        };
}
