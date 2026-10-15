using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class EnablePluginDoneReducer : IReducer<BootstrapState, EnablePluginAction>
{
    public BootstrapState Reduce(BootstrapState state, EnablePluginAction action)
        => state with
        {
        };
}
