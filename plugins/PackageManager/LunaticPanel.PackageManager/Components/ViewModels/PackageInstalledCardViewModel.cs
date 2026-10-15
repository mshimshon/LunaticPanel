using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageInstalledCardViewModel : WidgetViewModelBase, IPackageInstalledCardViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IPanelControl _panelControl;

    public PackagePayload Data { get; set; } = default!;
    public PackageUpdateState PackageUpdate => _statePulse.StateOf<PackageUpdateState>(() => this, UpdateChanges);
    public PackageManagerDiskState ManagerState => _statePulse.StateOf<PackageManagerDiskState>(() => this, UpdateChanges);
    public PackageUpdateScheduleState UpdateScheduleState => _statePulse.StateOf<PackageUpdateScheduleState>(() => this, UpdateChanges);
    public PackageManagerDiskEditorState ManagerEditorState => _statePulse.StateOf<PackageManagerDiskEditorState>(() => this, UpdateChanges);
    public BootstrapState BootstrapState => _statePulse.StateOf<BootstrapState>(() => this, UpdateChanges);

    public bool CanScheduleUpdate { get; private set; }
    public bool HasUpdateScheduled => ScheduledUpdate != default;

    public bool HasDeleteScheduled => ScheduledDelete != default;

    public bool CanScheduleRollback { get; private set; }
    public bool HasRollbackScheduled => ScheduledRollback != default;

    public PackagePayload? ScheduledUpdate { get; private set; }
    public PackagePayload? ScheduledDelete { get; private set; }
    public PackagePayload? ScheduledRollback { get; private set; }
    public bool RestartIsRequired { get; private set; }
    public bool CanDelete { get; private set; }
    public PackageManifestPayload? AvailableUpdate { get; private set; }
    public bool HasUpdateAvailable => AvailableUpdate != default;

    public PackagePayload? AvailableRollback { get; private set; }
    public bool IsPreInstalled { get; private set; }
    public bool IsAlreadyInstalled { get; private set; }
    public bool HasAvailableRollback => AvailableRollback != default;
    public PackageStatePayload State { get; set; } = PackageStatePayload.Unknown;
    public PackageStatePayload? PendingStateRestart { get; set; }
    public bool CanEnable { get; set; }
    public bool CanDisable { get; set; }
    public Task Restart()
    {
        _panelControl.Shutdown();
        return Task.CompletedTask;
    }
    public PackageInstalledCardViewModel(IStatePulse statePulse, IPanelControl panelControl)
    {
        _statePulse = statePulse;
        _panelControl = panelControl;
    }
    protected override void OnViewModelParametersSet()
    {
        IsPreInstalled = ManagerState.PreInstalled.Any(p => p.Info.PackageId == Data.Info.PackageId);
        IsAlreadyInstalled = ManagerState.Installed.Any(p => p.Info.PackageId == Data.Info.PackageId);
    }
    protected override bool GetStateLoadingStatus()
        => PackageUpdate.IsLoading || ManagerState.IsLoading || ManagerEditorState.IsLoading || BootstrapState.IsLoading;

    protected override void OnViewModelBeforeRender()
    {
        ScheduledUpdate = ManagerState.PendingUpdates.FirstOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        AvailableUpdate = PackageUpdate.FoundUpdates.FirstOrDefault(p => p.Id == Data.Info.PackageId);
        AvailableRollback = ManagerState.AvailableRollbacks.FirstOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        CanScheduleUpdate = !HasUpdateScheduled && HasUpdateAvailable;
        CanScheduleRollback = !HasUpdateScheduled && HasAvailableRollback && ScheduledUpdate != AvailableRollback;
        CanDelete = !IsPreInstalled;
        RestartIsRequired = !IsAlreadyInstalled && HasUpdateScheduled;
        CanEnable = (PendingStateRestart == default && State != PackageStatePayload.Enabled) || (PendingStateRestart != default && PendingStateRestart == PackageStatePayload.Disabled);
        CanDisable = (PendingStateRestart == default && State != PackageStatePayload.Disabled) || (PendingStateRestart != default && PendingStateRestart == PackageStatePayload.Enabled);
        var runtime = ManagerState.RuntimePackages.FirstOrDefault(p => p.Info.PackageId == Data.Info.PackageId);
        if (runtime != default)
            State = runtime.Info.State;
        if (!RestartIsRequired)
            if (BootstrapState.Enabled.Contains(Data.Info.PackageId) || State == PackageStatePayload.Disabled)
                PendingStateRestart = PackageStatePayload.Enabled;
            else if (BootstrapState.Disabled.Contains(Data.Info.PackageId) || State == PackageStatePayload.Enabled)
                PendingStateRestart = PackageStatePayload.Disabled;
            else
                PendingStateRestart = default;
    }

    public async Task UpdateAsync()
    {
        IsLoading = true;
        await _statePulse.Dispatcher.Prepare<PackageRollbackAction>().With(p => p.Package, Data).DispatchAsync();
        IsLoading = false;
    }
    public async Task CancelUpdateAsync() => await FailSafeExecutionAsync(CancelInstallAsync);
    public async Task CancelInstallAsync() => await FailSafeExecutionAsync(CancelUpdateAsync);
    public async Task RollbackAsync()
    {
        IsLoading = true;
        await _statePulse.Dispatcher.Prepare<PackageRollbackAction>()
            .With(p => p.Package, Data)
            .DispatchAsync();
        IsLoading = false;
    }
    public Task CancelScheduledRollbackAsync() => throw new NotImplementedException();
    public async Task Enable()
    {
        await _statePulse.Dispatcher.Prepare<EnablePluginAction>().With(p => p.Id, Data.Info.PackageId).DispatchAsync();
    }

    public async Task Disable()
    {
        await _statePulse.Dispatcher.Prepare<DisablePluginAction>().With(p => p.Id, Data.Info.PackageId).DispatchAsync();

    }
}
