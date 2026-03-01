using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Web.Services.Circuit;
using Microsoft.AspNetCore.Components;
namespace LunaticPanel.Engine.Web.Layout;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "SonarLint",
    "S3881:Implement the IDisposable pattern correctly",
    Justification = "Blazor components do not use the full dispose pattern.")]
public partial class MainLayout : LayoutComponentBase, IDisposable
{
    private readonly Guid _id = Guid.NewGuid();
    [Inject] public IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private CircuitRegistry CircuitRegistry { get; set; } = default!;
    private IEventBus EventBus { get; set; } = default!;

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
        }
    }

    protected override void OnInitialized()
    {
    }

    protected override void OnParametersSet()
    {
    }

    public void Dispose()
    {
        CircuitRegistry.SelfRemoval(_id, this);
    }
}
