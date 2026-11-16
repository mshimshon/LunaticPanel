using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class DisablePluginReducer : IReducer<BootstrapState, DisablePluginAction>
{
    public BootstrapState Reduce(BootstrapState state, DisablePluginAction action)
        => state with
        {
            IsLoading = true
        };
}
