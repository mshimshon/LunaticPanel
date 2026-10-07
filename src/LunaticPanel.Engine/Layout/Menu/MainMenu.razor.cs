using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Engine.Web.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LunaticPanel.Engine.Web.Layout.Menu;

public partial class MainMenu : ComponentBase, IAsyncDisposable
{


    [Inject] private MainMenuViewModel ViewModel { get; set; } = default!;
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

    private RenderFragment CreateRenderFragmentComponent(Type componentType)
    => componentType.CreateRenderFragmentComponent(RendererSetWidgetParameters);
    private void RendererSetWidgetParameters(RenderTreeBuilder builder)
    {
        builder.AddAttribute(1, nameof(WidgetComponentBase<>.OnParentStateHasChanged), EventCallback.Factory.Create(this, ShouldUpdate));

    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.LoadAsync();
        }
    }

    public ValueTask DisposeAsync()
    {
        ViewModel.SpreadChanges -= ShouldUpdate;
        return ValueTask.CompletedTask;
    }
}
