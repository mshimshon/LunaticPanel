using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class EnableSourceReducer : IReducer<RepositorySourceState, EnableSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, EnableSourceAction action)
        => state with
        {
            SourceSaving = true

        };
}
