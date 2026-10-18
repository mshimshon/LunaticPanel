using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageInstalledViewModel : WidgetViewModelBase, IPackageInstalledViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IEventBus _eventBus;
    private List<PackagePayload> _result = new List<PackagePayload>();

    public PackageManagerDiskState PackageManagerState => _statePulse.StateOf<PackageManagerDiskState>(() => this, UpdateChanges);
    private List<string> _enabledPlugins = new();
    private List<string> _disabledPlugins = new();
    public int InstalledPackageCount { get; private set; }

    public PackageUpdateState PackageUpdateState => _statePulse.StateOf<PackageUpdateState>(() => this, UpdateChanges);
    public BootstrapState BootstrapState => _statePulse.StateOf<BootstrapState>(() => this, UpdateChanges);
    public IEnumerable<PackagePayload> Result { get => _result; }

    public bool RestartRequired { get; set; }

    public PackageInstalledViewModel(IStatePulse statePulse, IEventBus eventBus)
    {
        _statePulse = statePulse;
        _eventBus = eventBus;
    }
    protected override async Task OnViewModelParametersSetAsync()
    {
        _enabledPlugins = PackageManagerState.RuntimePackages.Where(p => p.Info.State == Application.Payloads.Enums.PackageStatePayload.Enabled).Select(p => p.Info.PackageId).ToList();
        _disabledPlugins = PackageManagerState.RuntimePackages.Where(p => p.Info.State == Application.Payloads.Enums.PackageStatePayload.Disabled).Select(p => p.Info.PackageId).ToList();
    }

    protected override bool GetStateLoadingStatus() => PackageManagerState.IsLoading || PackageUpdateState.IsLoading || BootstrapState.IsLoading;
    protected override void OnViewModelBeforeRender()
    {
        InstalledPackageCount = PackageManagerState.Installed.Count();
        _result = PackageManagerState.Installed.ToList();
        RestartRequired = false;
        // Add Newly Installed 
        foreach (var item in PackageManagerState.PendingUpdates)
            if (!_result.Any(p => p.Info.PackageId == item.Info.PackageId))
            {
                RestartRequired = true;
                _result.Add(item);
            }
        if (!RestartRequired)
            RestartRequired = BootstrapState.Disabled.Any(_enabledPlugins.Contains);
        if (!RestartRequired)
            RestartRequired = BootstrapState.Enabled.Any(_disabledPlugins.Contains);
    }

    protected override async Task OnViewModelAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!PackageManagerState.IsInitialized)
                await _statePulse.Dispatcher.Prepare<LoadPackageDiskFoldersAction>().DispatchAsync();
        }
    }

    public async Task CheckForUpdates()
    {
        IsLoading = true;
        await _statePulse.Dispatcher.Prepare<PeriodicCheckPackageUpdatesAction>().DispatchAsync();
        IsLoading = false;
    }

    public async Task RestartPanel()
    {
        await _eventBus.PublishDatalessAsync(PanelKeys.Events.PanelShutdown);
    }
}
