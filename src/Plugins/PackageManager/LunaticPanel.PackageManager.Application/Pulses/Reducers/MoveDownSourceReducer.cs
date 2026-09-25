using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class MoveDownSourceReducer : IReducer<RepositorySourceState, MoveDownSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, MoveDownSourceAction action)
        => state with
        {
            SourceSaving = true

        };
}
