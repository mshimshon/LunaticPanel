using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Web.Services.Circuit;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Presentation.Layout;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "SonarLint",
    "S3881:Implement the IDisposable pattern correctly",
    Justification = "Blazor components do not use the full dispose pattern.")]
public partial class MainLayout : LayoutComponentBase, IDisposable
{
    private readonly Guid _id = Guid.NewGuid();
    [Inject] public IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private CircuitRegistry CircuitRegistry { get; set; } = default!;
    [Inject] private IEventBus EventBus { get; set; } = default!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            CircuitRegistry.SelfCircuitRegistration(_id, this);
            _ = EventBus.PublishDatalessAsync(DashboardKeys.Events.OnFirstRender);
        }
    }

    protected override void OnInitialized()
    {
        _ = EventBus.PublishDatalessAsync(DashboardKeys.Events.OnInitialized);
    }

    protected override void OnParametersSet()
    {
        _ = EventBus.PublishDatalessAsync(DashboardKeys.Events.OnParameterSet);
    }

    public void Dispose()
    {
        CircuitRegistry.SelfRemoval(_id, this);
    }
}
