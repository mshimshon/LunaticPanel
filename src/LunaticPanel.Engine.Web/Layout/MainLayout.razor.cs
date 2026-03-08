using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Web.Services.Circuit;
using Microsoft.AspNetCore.Components;
namespace LunaticPanel.Engine.Web.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    private readonly Guid _id = Guid.NewGuid();
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private CircuitRegistry CircuitRegistry { get; set; } = default!;
    private IEventBus EventBus { get; set; } = default!;
    [Inject] private MainLayoutViewModel ViewModel { get; set; } = default!;

    private Task ShouldUpdate() => InvokeAsync(StateHasChanged);
    protected override void OnInitialized()
    {
        ViewModel.SpreadChanges += ShouldUpdate;
    }
    protected override void OnParametersSet()
    {
    }
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            CircuitRegistry.SelfCircuitRegistration(_id, this);
            EventBus = ServiceProvider.GetRequiredService<IEventBus>();
            try
            {
                _ = EventBus.PublishDatalessAsync(DashboardKeys.Events.OnFirstRender);

            }
            catch (Exception ex)
            {

                throw;
            }
            InvokeAsync(StateHasChanged);
        }
    }




    public ValueTask DisposeAsync()
    {
        ViewModel.SpreadChanges -= ShouldUpdate;
        CircuitRegistry.SelfRemoval(_id, this);
        return ValueTask.CompletedTask;
    }
}
