using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class AddSourceDoneReducer : IReducer<RepositorySourceState, AddSourceDoneAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, AddSourceDoneAction action)
        => state with { SourceSaving = false };
}
