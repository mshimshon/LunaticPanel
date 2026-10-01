using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Engine.Web.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LunaticPanel.Engine.Web.Pages.Dashboard;

public partial class Dashboard : ComponentBase, IAsyncDisposable
{
    [Inject] private DashboardViewModel ViewModel { get; set; } = default!;
    [Inject] private IWidgetComponentLifecycle WidgetComponentLifecycle { get; set; } = default!;
    protected override void OnInitialized()
    {
        ViewModel.SpreadChanges += ShouldUpdate;
        WidgetComponentLifecycle.BringComponentAlive();
    }

    private async Task ShouldUpdate()
    {
        Console.WriteLine("DASHBOARD: RERENDER REQUESTED");
        await InvokeAsync(StateHasChanged);
    }

    private RenderFragment CreateRenderFragmentComponent(Type componentType)
        => componentType.CreateRenderFragmentComponent(RendererSetWidgetParameters);
    private void RendererSetWidgetParameters(RenderTreeBuilder builder)
    {
        builder.AddAttribute(1, nameof(WidgetComponentBase<>.OnParentStateHasChanged), EventCallback.Factory.Create(this, ShouldUpdate));

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await ViewModel.LoadAsync();
    }

    public ValueTask DisposeAsync()
    {
        ViewModel.SpreadChanges -= ShouldUpdate;
        WidgetComponentLifecycle.KillComponent();
        return ValueTask.CompletedTask;
    }
}
