using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class MoveDownSourceReducer : IReducer<RepositorySourceState, MoveDownSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveDownSourceAction action)
        => state with
        {
            SourceSaving = true

        };
}
