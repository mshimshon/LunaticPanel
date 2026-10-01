using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPackageInstalledCardViewModel : IWidgetViewModel
{
    PackagePayload Data { get; internal set; }
    PackageUpdateState PackageUpdate { get; }
    PackageManagerDiskState ManagerState { get; }
    PackageUpdateScheduleState UpdateScheduleState { get; }
    PackageManagerDiskEditorState ManagerEditorState { get; }
    PackagePayload? ScheduledUpdate { get; }
    PackagePayload? ScheduledDelete { get; }
    PackagePayload? ScheduledRollback { get; }
    PackagePayload? AvailableUpdate { get; }
    PackagePayload? AvailableRollback { get; }
    bool CanScheduleUpdate { get; }
    bool HasUpdateScheduled { get; }
    bool HasDeleteScheduled { get; }
    bool CanScheduleRollback { get; }
    bool HasRollbackScheduled { get; }
    bool CanDelete { get; }
    bool HasUpdateAvailable { get; }
    bool IsPreInstalled { get; }
    bool HasAvailableRollback { get; }
}
