using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class InstallPackageReducer : IReducer<PackageUpdateScheduleState, InstallPackageDoneAction>
{
    public PackageUpdateScheduleState Reduce(PackageUpdateScheduleState state, InstallPackageDoneAction action)
        => state with
        {
            Updating = true,
        };
}
