using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadSourcesReducer : IReducer<RepositorySourceState, LoadSourcesAction>
{
    public RepositorySourceState Reduce(RepositorySourceState state, LoadSourcesAction action)
  => state with
  {

      SourcesLoading = true
  };
}
