using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Web.Pages.Dashboard;

public partial class Dashboard : ComponentBase, IDisposable
{
    [Inject] private DashboardViewModel ViewModel { get; set; } = default!;
    protected override void OnInitialized()
    {
        ViewModel.SpreadChanges += ShouldUpdate;

    }
    private Task ShouldUpdate() => InvokeAsync(StateHasChanged);
    public void Dispose()
    {
        ViewModel.SpreadChanges -= ShouldUpdate;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.LoadAsync();
        }
    }
}
