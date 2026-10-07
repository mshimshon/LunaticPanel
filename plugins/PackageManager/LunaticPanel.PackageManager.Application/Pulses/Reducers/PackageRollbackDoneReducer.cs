using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PackageRollbackDoneReducer : IReducer<PackageManagerDiskEditorState, PackageRollbackDoneAction>
{
    public PackageManagerDiskEditorState Reduce(PackageManagerDiskEditorState state, PackageRollbackDoneAction action)
        => state with
        {
            IsLoading = false,
            PendingRollbacks = action.Packages.ToList()
        };
}
