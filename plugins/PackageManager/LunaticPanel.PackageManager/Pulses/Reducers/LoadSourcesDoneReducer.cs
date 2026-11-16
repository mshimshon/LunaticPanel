using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class LoadSourcesDoneReducer : IReducer<RepositorySourceState, LoadSourcesDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, LoadSourcesDoneAction action)
        => state with
        {
            Sources = action.Sources?.ToArray() ?? Array.Empty<RepositorySourcePayload>(),
            SourcesLoading = false
        };
}
