using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Pages.ViewModels;
using LunaticPanel.PackageManager.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pages;

internal class HomeViewModel : WidgetViewModelBase, IHomeViewModel
{
    private readonly IStatePulse _statePulse;
    public PackageInstallState InstallState => _statePulse.StateOf<PackageInstallState>(() => this, UpdateChanges);
    public HomeViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }
}
