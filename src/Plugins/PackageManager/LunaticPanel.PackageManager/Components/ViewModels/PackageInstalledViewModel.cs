using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageInstalledViewModel : WidgetViewModelBase, IPackageInstalledViewModel
{
    private readonly IStatePulse _statePulse;
    public PackageManagerState PackageManagerState => _statePulse.StateOf<PackageManagerState>(() => this, UpdateChanges);

    public int InstalledPackageCount { get; private set; }

    public PackageInstalledViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }
    protected override bool GetStateLoadingStatus() => PackageManagerState.IsPackageLoading;
    protected override void OnViewModelBeforeRender()
    {
        InstalledPackageCount = PackageManagerState.InstalledPackages.Count();
    }
    protected override async Task OnViewModelAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!PackageManagerState.IsPackageInitialized)
                await _statePulse.Dispatcher.Prepare<LoadLocalPackagesAction>().DispatchAsync();
        }
    }

}
