using LunaticPanel.Core.Abstraction.Widgets;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Web.Layout.Menu;

public partial class MainMenu : ComponentBase, IDisposable
{


    [Inject] public MainMenuViewModel ViewModel { get; set; } = default!;
    private Dictionary<string, object> TypeComponentParameters { get; set; } = new();
    protected override void OnInitialized()
    {
        ViewModel.SpreadChanges += ShouldUpdate;
        TypeComponentParameters = new()
        {
            { nameof(WidgetComponentBase<>.OnParentStateHasChanged), new EventCallbackFactory().Create(this, ShouldUpdate) },
        };
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
