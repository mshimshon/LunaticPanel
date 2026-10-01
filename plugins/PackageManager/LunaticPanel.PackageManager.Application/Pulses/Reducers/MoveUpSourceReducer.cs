using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class MoveUpSourceReducer : IReducer<RepositorySourceState, MoveUpSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveUpSourceAction action)
        => state with
        {
            SourceSaving = true

        };
}

