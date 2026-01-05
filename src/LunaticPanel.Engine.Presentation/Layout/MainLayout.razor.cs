using LunaticPanel.Engine.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Presentation.Layout;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    private readonly Guid _id = Guid.NewGuid();
    [Inject] public IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private CircuitRegistry CircuitRegistry { get; set; } = default!;


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            CircuitRegistry.SelfCircuitRegistration(_id, this);
    }

    protected override void OnInitialized()
    {

    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;
        CircuitRegistry.SelfRemoval(_id, this);

    }
}
