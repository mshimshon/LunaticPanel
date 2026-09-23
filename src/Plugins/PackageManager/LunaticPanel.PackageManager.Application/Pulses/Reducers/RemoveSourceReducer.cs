using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class RemoveSourceReducer : IReducer<RepositorySourceState, RemoveSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, RemoveSourceAction action)
        => state with
        {
            SourceSaving = true
        };
}
