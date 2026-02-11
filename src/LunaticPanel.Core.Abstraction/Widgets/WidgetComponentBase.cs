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
public abstract class WidgetComponentBase<TPluginEntry, TViewModel> : WidgetComponentBase<TPluginEntry>, IAsyncDisposable
    where TViewModel : IWidgetViewModel
    where TPluginEntry : IPlugin
{
    protected TViewModel ViewModel { get; private set; } = default!;
    private readonly SemaphoreSlim _renderGate = new(1, 1);
    public async Task OnViewModelChanged()
    {
        await InvokeAsync(async () =>
        {
            await _renderGate.WaitAsync();
            try
            {
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                _renderGate.Release();
            }
        });
    }
    private Func<Task>? _onVmChanged;
    protected override void OnBaseInitialized()
    {
        _onVmChanged = OnViewModelChanged;
        ViewModel = WidgetContext.GetViewModel<TViewModel>();
        ViewModel.SpreadChanges += _onVmChanged;
    }

    protected override void OnParametersSet()
    {
        OnWidgetParametersSet();
    }
    protected virtual void OnWidgetParametersSet() { }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender) { }
        OnWidgetAfterRender(firstRender);
    }
    protected virtual void OnWidgetAfterRender(bool firstRender) { }

    public async ValueTask DisposeAsync()
    {
        if (ViewModel is not null)
            ViewModel.SpreadChanges -= _onVmChanged;
        OnWidgetDispose();
        await OnWidgetDisposeAsync();
    }
    protected virtual void OnWidgetDispose() { }
    protected virtual Task OnWidgetDisposeAsync()
     => Task.CompletedTask;
}
