using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PeriodicCheckPackageUpdatesDoneReducer : IReducer<PackageUpdateState, PeriodicCheckPackageUpdatesDoneAction>
{
    public PackageUpdateState Reduce(PackageUpdateState state, PeriodicCheckPackageUpdatesDoneAction action)
        => state with
        {
            IsLoading = false,
            FoundUpdates = action.AvailableUpdates ?? Array.Empty<PackageManifestPayload>()
        };
}
