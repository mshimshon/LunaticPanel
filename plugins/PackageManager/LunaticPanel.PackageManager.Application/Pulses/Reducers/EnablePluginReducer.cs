using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class EnablePluginReducer : IReducer<BootstrapState, EnablePluginAction>
{
    public BootstrapState Reduce(BootstrapState state, EnablePluginAction action)
        => state with
        {
            IsLoading = true
        };
}
