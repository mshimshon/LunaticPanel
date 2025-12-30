using LunaticPanel.Engine.Presentation.Layout.Menu.ViewModels;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Presentation.Layout.Menu;

public partial class MainMenu : ComponentBase
{
    [Inject] public MainMenuViewModel ViewModel { get; set; } = default!;
    protected override void OnInitialized()
    {
        ViewModel.SpreadChanges = ShouldUpdate;
    }
    private Task ShouldUpdate() => InvokeAsync(StateHasChanged);
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.LoadAsync();
        }
    }

}
