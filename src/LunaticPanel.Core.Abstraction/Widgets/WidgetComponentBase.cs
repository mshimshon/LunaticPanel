using LunaticPanel.Core.Abstraction.Circuit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Abstraction.Widgets;

public abstract class WidgetComponentBase<TPluginEntry> : ComponentBase
        where TPluginEntry : IPlugin
{
    [Inject] protected IServiceProvider HostProvider { get; set; } = default!;
    protected IPluginContextService PluginContextService { get; private set; } = default!;
    protected IWidgetContext WidgetContext { get; private set; } = default!;

    protected override void OnInitialized()
    {
        var circuitRegistry = HostProvider.GetRequiredService<ICircuitRegistry>();
        PluginContextService = circuitRegistry.GetPluginContext(typeof(TPluginEntry).Namespace!, circuitRegistry.CurrentCircuit.CircuitId);
        WidgetContext = PluginContextService.GetRequired<IWidgetContext>();
        OnBaseInitialized();
        OnWidgetInitialized();
    }
    protected virtual void OnBaseInitialized() { }
    protected virtual void OnWidgetInitialized() { }

}

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "SonarLint",
    "S3881:Implement the IDisposable pattern correctly",
    Justification = "Blazor components do not use the full dispose pattern.")]
public abstract class WidgetComponentBase<TPluginEntry, TViewModel> : WidgetComponentBase<TPluginEntry>, IDisposable
    where TViewModel : IWidgetViewModel
    where TPluginEntry : IPlugin
{
    protected TViewModel ViewModel { get; private set; } = default!;

    public Task OnViewModelChanged() => InvokeAsync(StateHasChanged);
    protected override void OnBaseInitialized()
    {
        ViewModel = WidgetContext.GetViewModel<TViewModel>();
        ViewModel.SpreadChanges += OnViewModelChanged;
    }

    public void Dispose()
    {
        if (ViewModel is not null)
            ViewModel.SpreadChanges -= OnViewModelChanged;
    }
}
