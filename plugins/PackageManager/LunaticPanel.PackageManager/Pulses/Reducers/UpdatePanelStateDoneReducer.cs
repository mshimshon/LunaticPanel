using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class UpdatePanelStateDoneReducer : IReducer<PanelUpdateState, UpdatePanelStateDoneAction>
{
    public PanelUpdateState Reduce(PanelUpdateState state, UpdatePanelStateDoneAction action)
        => state with
        {
            AvailableVersion = action.Found,
            DownloadedVersion = action.Downloaded,
            StagedVersion = action.Staged,
        };
}
