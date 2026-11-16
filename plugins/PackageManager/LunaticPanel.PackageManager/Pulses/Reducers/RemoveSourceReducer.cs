using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class RemoveSourceReducer : IReducer<RepositorySourceState, RemoveSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, RemoveSourceAction action)
        => state with
        {
            SourceSaving = true
        };
}
