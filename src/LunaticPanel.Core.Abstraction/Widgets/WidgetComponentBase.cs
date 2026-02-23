using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Widgets.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Abstraction.Widgets;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "SonarLint",
    "S3881:Implement the IDisposable pattern correctly",
    Justification = "Blazor components do not use the full dispose pattern.")]
public abstract class WidgetComponentBase<TPluginEntry> : ComponentBase, IAsyncDisposable
        where TPluginEntry : IPlugin
{
    [Inject] protected IServiceProvider HostProvider { get; set; } = default!;
    protected IPluginContextService PluginContextService { get; private set; } = default!;
    protected IWidgetContext WidgetContext { get; private set; } = default!;

    [Parameter] public EventCallback OnParentStateHasChanged { get; set; }
    private readonly SemaphoreSlim _renderGate = new(1, 1);
    private readonly SemaphoreSlim _parentRenderGate = new(1, 1);


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


    protected sealed override void OnInitialized()
    {
        var circuitRegistry = HostProvider.GetRequiredService<ICircuitRegistry>();
        PluginContextService = circuitRegistry.GetPluginContext(typeof(TPluginEntry).Namespace!, circuitRegistry.CurrentCircuit.CircuitId);
        WidgetContext = PluginContextService.GetRequired<IWidgetContext>();
        BaseOnInitialized();
        OnWidgetInitialized();
    }
    protected sealed override async Task OnInitializedAsync()
    {
        await BaseOnInitializedAsync();
        await OnWidgetInitializedAsync();
    }
    protected virtual void BaseOnInitialized() { }
    protected virtual Task BaseOnInitializedAsync() => Task.CompletedTask;
    protected virtual void OnWidgetInitialized() { }
    protected virtual Task OnWidgetInitializedAsync() => Task.CompletedTask;

    protected sealed override void OnParametersSet()
    {
        BaseOnParametersSet();
        OnWidgetParametersSet();
    }
    protected sealed override async Task OnParametersSetAsync()
    {
        await BaseOnParametersSetAsync();
        await OnWidgetParametersSetAsync();

    }
    protected virtual void BaseOnParametersSet() { }
    protected virtual Task BaseOnParametersSetAsync() => Task.CompletedTask;
    protected virtual void OnWidgetParametersSet() { }
    protected virtual Task OnWidgetParametersSetAsync() => Task.CompletedTask;


    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        await BaseOnAfterRenderAsync(firstRender);
        await OnWidgetAfterRenderAsync(firstRender);
    }

    protected sealed override void OnAfterRender(bool firstRender)
    {
        BaseOnAfterRender(firstRender);
        OnWidgetAfterRender(firstRender);
    }
    protected virtual Task BaseOnAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void BaseOnAfterRender(bool firstRender) { }
    protected virtual void OnWidgetAfterRender(bool firstRender) { }
    protected virtual Task OnWidgetAfterRenderAsync(bool firstRender) => Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        BaseOnDispose();
        await BaseOnDisposeAsync();
        OnWidgetDispose();
        await OnWidgetDisposeAsync();
    }
    protected virtual void BaseOnDispose() { }
    protected virtual Task BaseOnDisposeAsync() => Task.CompletedTask;
    protected virtual void OnWidgetDispose() { }
    protected virtual Task OnWidgetDisposeAsync() => Task.CompletedTask;
}


public abstract class WidgetComponentBase<TPluginEntry, TViewModel> : WidgetComponentBase<TPluginEntry>, IAsyncDisposable
    where TViewModel : IWidgetViewModel
    where TPluginEntry : IPlugin
{
    protected TViewModel ViewModel { get; private set; } = default!;

    protected sealed override void BaseOnInitialized()
    {
        ViewModel = WidgetContext.GetViewModel<TViewModel>();
        ViewModel.SpreadChanges += InvokeStateChanges;
    }
    protected sealed override Task BaseOnInitializedAsync() => base.BaseOnInitializedAsync();

    protected sealed override void BaseOnAfterRender(bool firstRender) => base.BaseOnAfterRender(firstRender);
    protected sealed override Task BaseOnAfterRenderAsync(bool firstRender) => base.BaseOnAfterRenderAsync(firstRender);

    protected sealed override void BaseOnParametersSet() => base.BaseOnParametersSet();
    protected sealed override Task BaseOnParametersSetAsync() => base.BaseOnParametersSetAsync();

    protected override void BaseOnDispose()
    {
        if (ViewModel is not null)
            ViewModel.SpreadChanges -= InvokeStateChanges;
    }
    protected override Task BaseOnDisposeAsync() => base.BaseOnDisposeAsync();


}
