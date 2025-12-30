using LunaticPanel.Engine.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    private readonly Guid _id = Guid.NewGuid();
    [Inject] public IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private CircuitRegistry CircuitRegistry { get; set; } = default!;

    public ValueTask DisposeAsync()
    {
        CircuitRegistry.SelfRemoval(_id, this);
        return ValueTask.CompletedTask;
    }

    protected override void OnInitialized()
    {
    }
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            CircuitRegistry.SelfCircuitRegistration(_id, this);
    }


}
