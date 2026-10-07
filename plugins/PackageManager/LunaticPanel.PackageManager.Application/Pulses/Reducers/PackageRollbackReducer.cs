using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Reducers;

internal class PackageRollbackReducer : IReducer<PackageManagerDiskEditorState, PackageRollbackAction>
{
    public PackageManagerDiskEditorState Reduce(PackageManagerDiskEditorState state, PackageRollbackAction action)
        => state with
        {
            IsLoading = true
        };
}
