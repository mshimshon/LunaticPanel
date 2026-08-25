using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LunaticPanel.PackageManager.Components;

public partial class SourceAddFormDialog
{
    [CascadingParameter]
    public IMudDialogInstance MudDialog { get; set; } = default!;
    protected override void OnWidgetParametersSet()
    {
        ViewModel.MudDialog = MudDialog;
    }
}
