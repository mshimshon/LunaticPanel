using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class SearchRemotePackageReducer : IReducer<SearchPackageState, SearchRemotePackageAction>
{
    public SearchPackageState Reduce(SearchPackageState state, SearchRemotePackageAction action)
        => state with
        {
            IsLoading = true
        };
}
