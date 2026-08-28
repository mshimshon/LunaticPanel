using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPackageSearchCardViewModel : IWidgetViewModel
{
    PackageInfoPayload Data { get; set; }
    bool IsInstalled { get; set; }
    Task InstallAsync();
}
