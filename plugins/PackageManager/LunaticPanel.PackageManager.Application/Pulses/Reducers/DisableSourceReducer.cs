using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class DisableSourceReducer : IReducer<RepositorySourceState, DisableSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, DisableSourceAction action)
        => state with
        {
            SourceSaving = true
        };
}
