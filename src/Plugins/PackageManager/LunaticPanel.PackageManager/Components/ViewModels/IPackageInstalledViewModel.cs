using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPackageInstalledViewModel : IWidgetViewModel
{
    PackageManagerState PackageManagerState { get; }
    int InstalledPackageCount { get; }
}
