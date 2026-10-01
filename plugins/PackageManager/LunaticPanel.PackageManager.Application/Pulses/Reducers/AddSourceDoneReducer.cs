using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class AddSourceDoneReducer : IReducer<RepositorySourceState, AddSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, AddSourceDoneAction action)
        => state with { SourceSaving = false };
}
