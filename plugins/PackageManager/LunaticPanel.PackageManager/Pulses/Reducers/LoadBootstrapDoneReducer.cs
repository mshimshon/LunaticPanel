using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class LoadBootstrapDoneReducer : IReducer<BootstrapState, LoadBootstrapDoneAction>
{
    public BootstrapState Reduce(BootstrapState state, LoadBootstrapDoneAction action)
        => state with
        {
            IsLoading = false,
            Disabled = action.Disabled?.ToArray() ?? Array.Empty<string>(),
            Enabled = action.Enabled?.ToArray() ?? Array.Empty<string>()
        };
}
