using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class InstallNextUpdateDoneReducer : IReducer<PackageUpdateScheduleState, InstallNextUpdateDoneAction>
{
    public PackageUpdateScheduleState Reduce(PackageUpdateScheduleState state, InstallNextUpdateDoneAction action)
        => state with
        {
            CurrentlyUpdating = default,
            ToUpdate = action.ToRemove != default ?
                state.ToUpdate.Where(p => p.Info.PackageId != action.ToRemove.Info.PackageId).ToArray() :
                state.ToUpdate.ToArray()
        };
}
