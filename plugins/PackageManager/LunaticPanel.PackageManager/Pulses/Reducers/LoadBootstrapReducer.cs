using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class LoadBootstrapReducer : IReducer<BootstrapState, LoadBootstrapAction>
{
    public BootstrapState Reduce(BootstrapState state, LoadBootstrapAction action)
        => state with
        {
            IsLoading = true
        };
}
