using LunaticPanel.Engine.Presentation.Layout.Menu.ViewModels;
using Microsoft.AspNetCore.Components;
using SwizzleV;

namespace LunaticPanel.Engine.Presentation.Layout.Menu;

public partial class MainMenu : ComponentBase
{
    [Inject] public ISwizzleFactory SwizzleFactory { get; set; } = default!;
    private MainMenuViewModel _viewModel = default!;
    protected override void OnInitialized()
    {
        var articleVMHook = SwizzleFactory.CreateOrGet<MainMenuViewModel>(() => this, ShouldUpdate);
        _viewModel = articleVMHook.GetViewModel<MainMenuViewModel>()!;
    }
    private Task ShouldUpdate()
        => InvokeAsync(StateHasChanged);
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _viewModel.LoadAsync();
        }
    }

}
