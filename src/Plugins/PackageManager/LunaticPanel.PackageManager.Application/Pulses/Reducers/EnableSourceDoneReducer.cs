using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class EnableSourceDoneReducer : IReducer<RepositorySourceState, EnableSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, EnableSourceDoneAction action)
        => state with
        {
            SourceSaving = false

        };
}
