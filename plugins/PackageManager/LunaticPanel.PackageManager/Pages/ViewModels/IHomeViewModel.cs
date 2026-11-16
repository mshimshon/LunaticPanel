using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Pulses.States;

namespace LunaticPanel.PackageManager.Pages.ViewModels;

public interface IHomeViewModel : IWidgetViewModel
{
    PackageInstallState InstallState { get; }
}
