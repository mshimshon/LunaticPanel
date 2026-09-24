using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class DisableSourceDoneReducer : IReducer<RepositorySourceState, DisableSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, DisableSourceDoneAction action)
        => state with { 

        };
}
