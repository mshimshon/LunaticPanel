using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class LoadBootstrapReducer : IReducer<BootstrapState, LoadBootstrapAction>
{
    public BootstrapState Reduce(BootstrapState state, LoadBootstrapAction action)
        => state with
        {
            IsLoading = true
        };
}
