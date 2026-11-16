using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class RemoveSourceDoneReducer : IReducer<RepositorySourceState, RemoveSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, RemoveSourceDoneAction action)
        => state with
        {
            SourceSaving = false
        };
}
