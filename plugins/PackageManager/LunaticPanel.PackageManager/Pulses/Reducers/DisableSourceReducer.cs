using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class DisableSourceReducer : IReducer<RepositorySourceState, DisableSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, DisableSourceAction action)
        => state with
        {
            SourceSaving = true
        };
}
