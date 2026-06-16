using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class InstallNextUpdateReducer : IReducer<PackageUpdateScheduleState, InstallNextUpdateDoneAction>
{
    public PackageUpdateScheduleState Reduce(PackageUpdateScheduleState state, InstallNextUpdateDoneAction action)
        => state with
        {
            ToUpdate = action.ToRemove != default ? state.ToUpdate.Where(p => p.Info.PackageId != action.ToRemove.Info.PackageId) : state.ToUpdate,
            CurrentlyUpdating = default
        };
}
