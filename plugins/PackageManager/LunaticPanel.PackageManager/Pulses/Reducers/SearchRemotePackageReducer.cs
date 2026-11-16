using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class SearchRemotePackageReducer : IReducer<SearchPackageState, SearchRemotePackageAction>
{
    public SearchPackageState Reduce(SearchPackageState state, SearchRemotePackageAction action)
        => state with
        {
            IsLoading = true
        };
}
