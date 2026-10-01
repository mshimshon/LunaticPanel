using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadSourcesDoneReducer : IReducer<RepositorySourceState, LoadSourcesDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, LoadSourcesDoneAction action)
        => state with
        {
            Sources = action.Sources?.ToArray() ?? Array.Empty<RepositorySourcePayload>(),
            SourcesLoading = false
        };
}
