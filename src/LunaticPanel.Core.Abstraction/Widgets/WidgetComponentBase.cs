using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Widgets.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Abstraction.Widgets;

public abstract class WidgetComponentBase<TPluginEntry> : ComponentBase
        where TPluginEntry : IPlugin
{
    [Inject] protected IServiceProvider HostProvider { get; set; } = default!;
    protected IPluginContextService PluginContextService { get; private set; } = default!;
    protected IWidgetContext WidgetContext { get; private set; } = default!;

    [Parameter] public EventCallback OnParentStateHasChanged { get; set; }
    private readonly SemaphoreSlim _renderGate = new(1, 1);
    private readonly SemaphoreSlim _parentRenderGate = new(1, 1);

    protected override void OnInitialized()
    {
        var circuitRegistry = HostProvider.GetRequiredService<ICircuitRegistry>();
        PluginContextService = circuitRegistry.GetPluginContext(typeof(TPluginEntry).Namespace!, circuitRegistry.CurrentCircuit.CircuitId);
        WidgetContext = PluginContextService.GetRequired<IWidgetContext>();
        OnBaseInitialized();
        OnWidgetInitialized();
    }
    protected async Task InvokeParentStateChanged()
    {
        if (OnParentStateHasChanged.HasDelegate)
            await InvokeAsync(async () =>
            {
                await _parentRenderGate.WaitAsync();
                try
                {
                    await OnParentStateHasChanged.InvokeAsync();
                }
                catch (Exception ex)
                {
                    // TODO: LOG ISSUES
                    Console.WriteLine($"WidgetComponentBase::InvokeParentStateChanged = {ex.Message}");
                }
                finally
                {
                    _parentRenderGate.Release();
                }
            });
    }
    protected async Task InvokeMyComponentStateChanged()
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
                // TODO: LOG ISSUES
                Console.WriteLine($"WidgetComponentBase::InvokeMyComponentStateChanged = {ex.Message}");
            }
            finally
            {
                _renderGate.Release();
            }
        });
    }

    protected virtual Task InvokeStateChanges(SpreadChangeOption spreadChangeOption = SpreadChangeOption.TouchMyComponentOnly)
    {
        if (spreadChangeOption == SpreadChangeOption.TouchMyComponentOnly)
            return InvokeMyComponentStateChanged();
        else
            return InvokeParentStateChanged();
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



    protected override void OnBaseInitialized()
    {
        ViewModel = WidgetContext.GetViewModel<TViewModel>();
        ViewModel.SpreadChanges += InvokeStateChanges;
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
            ViewModel.SpreadChanges -= InvokeStateChanges;
        OnWidgetDispose();
        await OnWidgetDisposeAsync();
    }
    protected virtual void OnWidgetDispose() { }
    protected virtual Task OnWidgetDisposeAsync()
     => Task.CompletedTask;
}
