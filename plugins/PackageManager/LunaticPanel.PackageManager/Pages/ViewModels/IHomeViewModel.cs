using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Pages.ViewModels;

public interface IHomeViewModel : IWidgetViewModel
{
    PackageInstallState InstallState { get; }
}
