using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class MoveDownSourceDoneReducer : IReducer<RepositorySourceState, MoveDownSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveDownSourceDoneAction action)
        => state with
        {
            SourceSaving = false

        };
}
