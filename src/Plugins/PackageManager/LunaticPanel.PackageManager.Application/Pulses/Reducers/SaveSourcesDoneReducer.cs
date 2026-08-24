using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class SaveSourcesDoneReducer : IReducer<RepositorySourceState, SaveSourcesDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, SaveSourcesDoneAction action)
        => state with
        {
            SourceSaving = false,
            Sources = action.Sources ?? state.Sources
        };
}
