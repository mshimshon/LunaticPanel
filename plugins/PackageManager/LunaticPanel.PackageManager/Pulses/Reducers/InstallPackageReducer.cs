using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class InstallPackageReducer : IReducer<PackageUpdateScheduleState, InstallPackageDoneAction>
{
    public PackageUpdateScheduleState Reduce(PackageUpdateScheduleState state, InstallPackageDoneAction action)
        => state with
        {
        };
}
