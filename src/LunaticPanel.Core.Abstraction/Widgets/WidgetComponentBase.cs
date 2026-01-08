using LunaticPanel.Core.Abstraction.Circuit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Abstraction.Widgets;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "SonarLint",
    "S3881:Implement the IDisposable pattern correctly",
    Justification = "Blazor components do not use the full dispose pattern.")]
public abstract class WidgetComponentBase<TPluginEntry, TViewModel> : ComponentBase, IDisposable
    where TViewModel : IWidgetViewModel
    where TPluginEntry : IPlugin
{
    protected IPluginContextService PluginContextService { get; private set; } = default!;
    protected TViewModel ViewModel { get; private set; } = default!;
    [Inject] private IServiceProvider HostProvider { get; set; } = default!;

    public Task OnViewModelChanged() => InvokeAsync(StateHasChanged);
    protected override void OnInitialized()
    {
        var circuitRegistry = HostProvider.GetRequiredService<ICircuitRegistry>();
        PluginContextService = circuitRegistry.GetPluginContext(typeof(TPluginEntry).Namespace!, circuitRegistry.CurrentCircuit.CircuitId);
        var widgetContext = PluginContextService.GetRequired<IWidgetContext>();
        ViewModel = widgetContext.GetViewModel<TViewModel>();
        ViewModel.SpreadChanges += OnViewModelChanged;
        ThenInitialized();
    }

    protected virtual void ThenInitialized()
    {

    }
    public void Dispose()
    {
        if (ViewModel is not null)
            ViewModel.SpreadChanges -= OnViewModelChanged;
    }
}
