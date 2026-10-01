using LunaticPanel.PackageManager.Application.Payloads;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.PackageManager.Components;

public partial class SourceManagerCard
{

    [Parameter] public RepositorySourcePayload Item { get; set; } = default!;

    protected override void OnWidgetParametersSet()
    {
        ViewModel.Item = Item;
    }



}
