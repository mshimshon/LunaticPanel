using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Presentation.Pages.Dashboard;

public partial class Dashboard : ComponentBase
{
    [Inject] private DashboardViewModel ViewModel { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Console.WriteLine("sdaS");
            await ViewModel.LoadAsync();
        }
    }
}
