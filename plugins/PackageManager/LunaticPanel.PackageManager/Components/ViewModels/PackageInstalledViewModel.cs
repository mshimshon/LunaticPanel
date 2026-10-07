using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageInstalledViewModel : WidgetViewModelBase, IPackageInstalledViewModel
{
    private readonly IStatePulse _statePulse;
    private List<PackagePayload> _result = new List<PackagePayload>();

    public PackageManagerDiskState PackageManagerState => _statePulse.StateOf<PackageManagerDiskState>(() => this, UpdateChanges);

    public int InstalledPackageCount { get; private set; }

    public PackageUpdateState PackageUpdateState => _statePulse.StateOf<PackageUpdateState>(() => this, UpdateChanges);
    public IEnumerable<PackagePayload> Result { get => _result; }
    public PackageInstalledViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }
    protected override bool GetStateLoadingStatus() => PackageManagerState.IsLoading;
    protected override void OnViewModelBeforeRender()
    {
        InstalledPackageCount = PackageManagerState.Installed.Count();
        _result = PackageManagerState.Installed.ToList();
        // Add Newly Installed 
        foreach (var item in PackageManagerState.PendingUpdates)
            if (!_result.Any(p => p.Info.PackageId == item.Info.PackageId))
                _result.Add(item);
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
}
