using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class SearchRemotePackageDoneReducer : IReducer<SearchPackageState, SearchRemotePackageDoneAction>
{
    public SearchPackageState Reduce(SearchPackageState state, SearchRemotePackageDoneAction action)
        => state with
        {
            IsLoading = false,
            Search = action.Result
        };
}
