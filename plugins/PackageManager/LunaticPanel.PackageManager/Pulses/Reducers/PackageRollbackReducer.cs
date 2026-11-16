using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Reducers;

internal class PackageRollbackReducer : IReducer<PackageManagerDiskEditorState, PackageRollbackAction>
{
    public PackageManagerDiskEditorState Reduce(PackageManagerDiskEditorState state, PackageRollbackAction action)
        => state with
        {
            IsLoading = true
        };
}
