using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PackageRollbackDoneReducer : IReducer<PackageManagerDiskEditorState, PackageRollbackDoneAction>
{
    public PackageManagerDiskEditorState Reduce(PackageManagerDiskEditorState state, PackageRollbackDoneAction action)
        => state with
        {
            IsLoading = false,
            PendingRollbacks = action.Packages?.ToArray() ?? Array.Empty<PackagePayload>()
        };
}
