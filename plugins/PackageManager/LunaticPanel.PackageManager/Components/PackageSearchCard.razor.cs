using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Components.ViewModels;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.PackageManager.Components;

public partial class PackageSearchCard : WidgetComponentBase<PluginEntry, IPackageSearchCardViewModel>
{
    [Parameter] public PackageInfoPayload Data { get; set; } = default!;
    protected override void OnWidgetParametersSet()
    {
        ViewModel.Data = Data;
    }
}
