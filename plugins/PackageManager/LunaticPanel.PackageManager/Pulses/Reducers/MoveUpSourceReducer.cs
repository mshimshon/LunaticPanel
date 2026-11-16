using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class MoveUpSourceReducer : IReducer<RepositorySourceState, MoveUpSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveUpSourceAction action)
        => state with
        {
            SourceSaving = true

        };
}

