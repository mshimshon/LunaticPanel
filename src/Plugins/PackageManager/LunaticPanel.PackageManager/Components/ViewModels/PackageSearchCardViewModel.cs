using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageSearchCardViewModel : WidgetViewModelBase, IPackageSearchCardViewModel
{
    private readonly IStatePulse _statePulse;

    public PackageManagerState ManagerState => _statePulse.StateOf<PackageManagerState>(() => this, UpdateChanges);

    public PackageInfoPayload Data { get; set; } = default!;
    public bool IsInstalled { get; set; }
    public PackageSearchCardViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }

    protected override void OnViewModelBeforeRender()
    {
        IsInstalled = ManagerState.InstalledPackages.Any(p => p.Info.PackageId == Data.PackageId);
    }
}
