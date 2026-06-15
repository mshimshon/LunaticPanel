using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.States.Models;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPackageInstalledCardViewModel : IWidgetViewModel
{
    PackageLocalPulseModel DataModel { get; set; }
    bool HasUpdateAvailable { get; }
    bool HasUpdateScheduled { get; }
    bool CheckingForUpdate { get; }
    bool CanScheduleUpdate { get; }
    bool CanCancelScheduledUpdate { get; }
    bool CanScheduleRollback { get; }
    bool CanCancelScheduledRollback { get; }
    Task ScheduledUpdateAsync();
    Task CancelScheduledUpdateAsync();
    Task ScheduledRollbackAsync();
    Task CancelScheduledRollbackAsync();
}
