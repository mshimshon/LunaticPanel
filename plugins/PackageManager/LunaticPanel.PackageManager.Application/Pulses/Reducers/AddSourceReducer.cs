using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class AddSourceReducer : IReducer<RepositorySourceState, AddSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, AddSourceAction action)
        => state with { SourcesLoading = true };
}
