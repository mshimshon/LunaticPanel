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

    [Parameter] public EventCallback OnParentStateHasChanged { get; set; }
    private readonly SemaphoreSlim _renderGate = new(1, 1);
    private readonly SemaphoreSlim _parentRenderGate = new(1, 1);

    protected bool FirstRenderCompleted { get; private set; }
    protected async Task InvokeParentStateChanged()
    {
        await InvokeMyComponentStateChanged();
        if (OnParentStateHasChanged.HasDelegate)
            await InvokeAsync(async () =>
            {
                await _parentRenderGate.WaitAsync();
                try
                {
                    Console.WriteLine($"InvokeParentStateChanged:: Parent Rerender");
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
        if (firstRender)
            FirstRenderCompleted = true;
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
    protected IWidgetContext WidgetContext { get; private set; } = default!;
    protected TViewModel ViewModel { get; private set; } = default!;
    private IWidgetLifecycleViewModel? _widgetLifecycle;
    protected sealed override void BaseOnInitialized()
    {
        base.BaseOnInitialized();
        WidgetContext = PluginContextService.GetRequired<IWidgetContext>();
        ViewModel = WidgetContext.GetViewModel<TViewModel>();
        try
        {
            _widgetLifecycle = (IWidgetLifecycleViewModel)ViewModel;
        }
        catch
        {
            Console.WriteLine("{0} underlaying type does not inherit {1}, it is not mandatory but will disabled internal features.",
                typeof(TViewModel).Name,
                typeof(WidgetViewModelBase).Name);
        }
        ViewModel.SpreadChanges += InvokeStateChanges;
        if (_widgetLifecycle != default)
            _widgetLifecycle.OnInitialized();
    }
    protected sealed override async Task BaseOnInitializedAsync()
    {
        await base.BaseOnInitializedAsync();
        if (_widgetLifecycle != default)
            await _widgetLifecycle.OnInitializedAsync();
    }

    protected sealed override void BaseOnAfterRender(bool firstRender)
    {
        base.BaseOnAfterRender(firstRender);
        if (_widgetLifecycle != default)
            _widgetLifecycle.OnAfterRender(firstRender);
    }
    protected sealed override async Task BaseOnAfterRenderAsync(bool firstRender)
    {
        await base.BaseOnAfterRenderAsync(firstRender);
        if (_widgetLifecycle != default)
            await _widgetLifecycle.OnAfterRenderAsync(firstRender);
    }

    protected sealed override void BaseOnParametersSet()
    {
        base.BaseOnParametersSet();
        if (_widgetLifecycle != default)
            _widgetLifecycle.OnParametersSet();
    }
    protected sealed override async Task BaseOnParametersSetAsync()
    {
        await base.BaseOnParametersSetAsync();
        if (_widgetLifecycle != default)
            await _widgetLifecycle.OnParametersSetAsync();
    }

    protected sealed override void BaseOnDispose()
    {
        if (ViewModel is not null)
            ViewModel.SpreadChanges -= InvokeStateChanges;
    }
    protected sealed override Task BaseOnDisposeAsync() => base.BaseOnDisposeAsync();


}
