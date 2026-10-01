using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class MoveDownSourceDoneReducer : IReducer<RepositorySourceState, MoveDownSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveDownSourceDoneAction action)
        => state with
        {
            SourceSaving = false

        };
}
