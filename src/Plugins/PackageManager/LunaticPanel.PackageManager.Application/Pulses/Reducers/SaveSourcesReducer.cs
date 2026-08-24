using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class SaveSourcesReducer : IReducer<RepositorySourceState, SaveSourcesAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, SaveSourcesAction action)
        => state with { SourceSaving = true };
}
