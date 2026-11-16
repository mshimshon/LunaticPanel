using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class AddSourceReducer : IReducer<RepositorySourceState, AddSourceAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, AddSourceAction action)
        => state with { SourcesLoading = true };
}
