using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class DisablePluginDoneReducer : IReducer<BootstrapState, DisablePluginAction>
{
    public BootstrapState Reduce(BootstrapState state, DisablePluginAction action)
        => state with
        {
        };
}
