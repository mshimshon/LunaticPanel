using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class MoveUpSourceDoneReducer : IReducer<RepositorySourceState, MoveUpSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveUpSourceDoneAction action)
        => state with
        {
            SourceSaving = false

        };
}

