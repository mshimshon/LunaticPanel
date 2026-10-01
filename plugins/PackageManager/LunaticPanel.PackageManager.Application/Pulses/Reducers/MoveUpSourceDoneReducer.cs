using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class MoveUpSourceDoneReducer : IReducer<RepositorySourceState, MoveUpSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveUpSourceDoneAction action)
        => state with
        {
            SourceSaving = false

        };
}

