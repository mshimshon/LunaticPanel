using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageInstalledCardViewModel : WidgetViewModelBase, IPackageInstalledCardViewModel
{
    private readonly IStatePulse _statePulse;

    public PackagePayload Data { get; set; } = default!;
    public PackageManagerState ManagerState => _statePulse.StateOf<PackageManagerState>(() => this, UpdateChanges);
    public PackageUpdateScheduleState UpdateScheduleState => _statePulse.StateOf<PackageUpdateScheduleState>(() => this, UpdateChanges);
    public bool HasUpdateAvailable { get; private set; }
    public bool HasUpdateScheduled { get; private set; }
    public bool CheckingForUpdate { get; private set; }
    public bool CanScheduleUpdate { get; private set; }
    public bool CanCancelScheduledUpdate { get; private set; }
    public PackagePayload? ScheduledUpdate { get; private set; }
    public PackagePayload? AvailableUpdate { get; private set; }
    public PackagePayload? AvailableRollback { get; private set; }
    public bool HasRollbackAvailable { get; private set; }
    public bool HasRollbackScheduled { get; private set; }
    public bool CanScheduleRollback { get; private set; }

    public bool CanCancelScheduledRollback { get; private set; }

    public PackageInstalledCardViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }

    protected override void OnViewModelParametersSet()
    {

    }

    protected override void OnViewModelBeforeRender()
    {
        ScheduledUpdate = UpdateScheduleState.ToUpdate.SingleOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        AvailableUpdate = ManagerState.AvailableUpdatePackages.SingleOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        AvailableRollback = ManagerState.AvailableRollbackPackages.SingleOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        HasUpdateAvailable = AvailableUpdate != default && AvailableUpdate?.Version != Data.Version;
        HasRollbackAvailable = AvailableRollback != default && AvailableRollback?.Version != Data.Version;
        HasUpdateScheduled = AvailableUpdate != default && UpdateScheduleState.CurrentlyUpdating != default && UpdateScheduleState.CurrentlyUpdating.Info.PackageId == AvailableUpdate.Info.PackageId ||
            AvailableUpdate != default && ScheduledUpdate != default && AvailableUpdate.Info.PackageId == ScheduledUpdate.Info.PackageId;

        CheckingForUpdate = ManagerState.IsUpdateLoading;
        CanScheduleUpdate = !HasUpdateScheduled && HasUpdateAvailable;
        CanScheduleRollback = !HasUpdateScheduled && HasRollbackAvailable;
        CanCancelScheduledRollback = HasUpdateScheduled && ScheduledUpdate == AvailableRollback;
        CanCancelScheduledUpdate = HasUpdateScheduled && ScheduledUpdate == AvailableUpdate;
    }

    public Task ScheduledUpdateAsync() => throw new NotImplementedException();
    public Task CancelScheduledUpdateAsync() => throw new NotImplementedException();
    public Task ScheduledRollbackAsync() => throw new NotImplementedException();
    public Task CancelScheduledRollbackAsync() => throw new NotImplementedException();
}
