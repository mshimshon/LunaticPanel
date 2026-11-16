using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class LoadSourcesReducer : IReducer<RepositorySourceState, LoadSourcesAction>
{
  public RepositorySourceState Reduce(RepositorySourceState state, LoadSourcesAction action)
=> state with
{

  SourcesLoading = true
};
}
