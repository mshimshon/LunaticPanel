using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class RemoveSourceDoneReducer : IReducer<RepositorySourceState, RemoveSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, RemoveSourceDoneAction action)
        => state with
        {
            SourceSaving = false
        };
}
