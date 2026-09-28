using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PeriodicCheckPackageUpdatesDoneReducer : IReducer<PackageUpdateState, PeriodicCheckPackageUpdatesDoneAction>
{
    public PackageUpdateState Reduce(PackageUpdateState state, PeriodicCheckPackageUpdatesDoneAction action)
        => state with
        {
            IsLoading = false,
            FoundUpdates = action.AvailableUpdates.ToList()
        };
}
