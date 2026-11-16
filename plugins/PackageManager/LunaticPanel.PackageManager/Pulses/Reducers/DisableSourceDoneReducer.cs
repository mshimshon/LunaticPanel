using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class DisableSourceDoneReducer : IReducer<RepositorySourceState, DisableSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, DisableSourceDoneAction action)
        => state with
        {
            SourceSaving = false

        };
}
