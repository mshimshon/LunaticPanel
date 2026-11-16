using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class EnableSourceDoneReducer : IReducer<RepositorySourceState, EnableSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, EnableSourceDoneAction action)
        => state with
        {
            SourceSaving = false

        };
}
