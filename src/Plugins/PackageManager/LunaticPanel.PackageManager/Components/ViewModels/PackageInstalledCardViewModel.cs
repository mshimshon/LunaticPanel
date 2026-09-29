using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageInstalledCardViewModel : WidgetViewModelBase, IPackageInstalledCardViewModel
{
    private readonly IStatePulse _statePulse;

    public PackagePayload Data { get; set; } = default!;
    public PackageUpdateState PackageUpdate => _statePulse.StateOf<PackageUpdateState>(() => this, UpdateChanges);
    public PackageManagerDiskState ManagerState => _statePulse.StateOf<PackageManagerDiskState>(() => this, UpdateChanges);
    public PackageUpdateScheduleState UpdateScheduleState => _statePulse.StateOf<PackageUpdateScheduleState>(() => this, UpdateChanges);
    public PackageManagerDiskEditorState ManagerEditorState => _statePulse.StateOf<PackageManagerDiskEditorState>(() => this, UpdateChanges);

    public bool CanScheduleUpdate { get; private set; }
    public bool HasUpdateScheduled => ScheduledUpdate != default;

    public bool HasDeleteScheduled => ScheduledDelete != default;

    public bool CanScheduleRollback { get; private set; }
    public bool HasRollbackScheduled => ScheduledRollback != default;

    public PackagePayload? ScheduledUpdate { get; private set; }
    public PackagePayload? ScheduledDelete { get; private set; }
    public PackagePayload? ScheduledRollback { get; private set; }
    public bool CanDelete { get; private set; }
    public PackagePayload? AvailableUpdate { get; private set; }
    public bool HasUpdateAvailable => AvailableUpdate != default;

    public PackagePayload? AvailableRollback { get; private set; }
    public bool IsPreInstalled { get; private set; }
    public bool HasAvailableRollback => AvailableRollback != default;


    public PackageInstalledCardViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }

    protected override void OnViewModelParametersSet()
    {
        IsPreInstalled = ManagerState.PreInstalled.Any(p => p.Info.PackageId == Data.Info.PackageId);
    }

    protected override void OnViewModelBeforeRender()
    {
        ScheduledUpdate = ManagerState.PendingUpdates.FirstOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        AvailableUpdate = PackageUpdate.FoundUpdates.FirstOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        AvailableRollback = ManagerState.AvailableRollbacks.FirstOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        CanScheduleUpdate = !HasUpdateScheduled && HasUpdateAvailable;
        CanScheduleRollback = !HasUpdateScheduled && HasAvailableRollback && ScheduledUpdate != AvailableRollback;
        CanDelete = !IsPreInstalled;
    }

    public async Task UpdateAsync()
    {
        IsLoading = true;
        await _statePulse.Dispatcher.Prepare<PackageRollbackAction>().With(p => p.Package, Data).DispatchAsync();
        IsLoading = false;
    }
    public async Task CancelUpdateAsync() => await CancelInstallAsync();
    public async Task CancelInstallAsync() => await CancelUpdateAsync();
    public async Task RollbackAsync()
    {
        IsLoading = true;
        await _statePulse.Dispatcher.Prepare<PackageRollbackAction>()
            .With(p => p.Package, Data)
            .DispatchAsync();
        IsLoading = false;
    }
    public Task CancelScheduledRollbackAsync() => throw new NotImplementedException();
}
