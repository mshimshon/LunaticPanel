using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPackageInstalledCardViewModel : IWidgetViewModel
{
    PackagePayload Data { get; set; }
    PackageManagerState ManagerState { get; }
    PackageUpdateScheduleState UpdateScheduleState { get; }
    bool HasUpdateAvailable { get; }
    bool HasUpdateScheduled { get; }
    bool CheckingForUpdate { get; }
    bool CanScheduleUpdate { get; }
    bool CanCancelScheduledUpdate { get; }
    PackagePayload? ScheduledUpdate { get; }
    PackagePayload? AvailableUpdate { get; }
    PackagePayload? AvailableRollback { get; }
    bool HasRollbackAvailable { get; }
    bool HasRollbackScheduled { get; }
    bool CanScheduleRollback { get; }
    bool CanCancelScheduledRollback { get; }
    Task ScheduledUpdateAsync();
    Task CancelScheduledUpdateAsync();
    Task ScheduledRollbackAsync();
    Task CancelScheduledRollbackAsync();
}
