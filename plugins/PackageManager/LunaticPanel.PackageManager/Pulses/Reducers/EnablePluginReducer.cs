using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class EnablePluginReducer : IReducer<BootstrapState, EnablePluginAction>
{
    public BootstrapState Reduce(BootstrapState state, EnablePluginAction action)
        => state with
        {
            IsLoading = true
        };
}
