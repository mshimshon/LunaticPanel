using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class SearchRemotePackageDoneReducer : IReducer<SearchPackageState, SearchRemotePackageDoneAction>
{
    public SearchPackageState Reduce(SearchPackageState state, SearchRemotePackageDoneAction action)
        => state with
        {
            IsLoading = false,
            Search = action.Result.ToDictionary() // Clean Copy
        };
}
