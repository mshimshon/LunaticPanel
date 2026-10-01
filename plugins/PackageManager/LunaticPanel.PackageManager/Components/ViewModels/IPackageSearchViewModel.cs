using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPackageSearchViewModel : IWidgetViewModel
{
    SearchPackageState SearchPackageState { get; }
    IEnumerable<PackageInfoPayload> SearchResult { get; }
    string Keywords { get; set; }
}
