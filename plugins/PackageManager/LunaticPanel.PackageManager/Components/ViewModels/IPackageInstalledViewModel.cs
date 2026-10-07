using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPackageInstalledViewModel : IWidgetViewModel
{
    int InstalledPackageCount { get; }
    PackageManagerDiskState PackageManagerState { get; }
    PackageUpdateState PackageUpdateState { get; }
    IEnumerable<PackagePayload> Result { get; }
    Task CheckForUpdates();
}
