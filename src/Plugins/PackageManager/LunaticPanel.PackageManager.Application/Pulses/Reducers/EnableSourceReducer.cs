using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class EnableSourceReducer : IReducer<RepositorySourceState, EnableSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, EnableSourceAction action)
        => state with
        {
            SourceSaving = true

        };
}
