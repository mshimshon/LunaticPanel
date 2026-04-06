using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Plugin;
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
    [Inject] private IWidgetComponentLifecycle WidgetComponentLifecycle { get; set; } = default!;
    protected IPluginContextService PluginContextService { get; private set; } = default!;

    [Parameter] public EventCallback OnParentStateHasChanged { get; set; }
    private readonly SemaphoreSlim _renderGate = new(1, 1);
    private readonly SemaphoreSlim _parentRenderGate = new(1, 1);
    private bool _renderCoalescing;
    private object _lock = new object();
    private bool _renderParentCoalescing;
    private object _lockParentCoalescing = new object();
    protected bool FirstRenderCompleted { get; private set; }


    private bool _disposed = false;
    private readonly SemaphoreSlim _disposeLock = new(1, 1);
    protected sealed override void OnInitialized()
    {
        var circuitRegistry = HostProvider.GetRequiredService<ICircuitRegistry>();
        PluginContextService = circuitRegistry.GetPluginContext(typeof(TPluginEntry).Namespace!, circuitRegistry.CurrentCircuit.CircuitId);
        BaseOnInitialized();
        OnWidgetInitialized();
    }

    protected async Task InvokeParentStateChanged()
    {

        await InvokeMyComponentStateChanged();
        lock (_lockParentCoalescing)
        {
            if (_renderParentCoalescing) return;
            _renderParentCoalescing = true;
        }
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
                    lock (_lock)
                    {
                        _renderParentCoalescing = false;
                    }
                    _parentRenderGate.Release();
                }
            });

    }
    protected async Task InvokeMyComponentStateChanged()
    {
        lock (_lock)
        {
            if (_renderCoalescing) return;
            _renderCoalescing = true;
        }
        await InvokeAsync(async () =>
        {
            // TODO: Review if ditching render gate.
            await _renderGate.WaitAsync();
            try
            {
                SafeInvokeAsync(BaseOnBeforeRender);
                await SafeInvokeAsync(BaseOnBeforeRenderAsync);
                SafeInvokeAsync(OnWidgetBeforeRender);
                await SafeInvokeAsync(OnWidgetBeforeRenderAsync);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                // TODO: LOG ISSUES
                Console.WriteLine($"WidgetComponentBase::InvokeMyComponentStateChanged = {ex.Message}");
            }
            finally
            {
                lock (_lock)
                {
                    _renderCoalescing = false;
                }
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
    private static async Task SafeInvokeAsync(Func<Task> asyncMethod)
    {
        try
        {
            var task = asyncMethod();
            if (task != null)
                await task;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WidgetComponentBase:: {ex.Message}");
            // TODO: LOG
        }
    }
    private static void SafeInvokeAsync(Action method)
    {
        try
        {
            method();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WidgetComponentBase:: {ex.Message}");
            // TODO: LOG
        }
    }



    protected sealed override async Task OnInitializedAsync()
    {
        await WidgetComponentLifecycle.BringComponentAlive();
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
        BaseOnBeforeRender();
        OnWidgetBeforeRender();
    }

    protected sealed override async Task OnParametersSetAsync()
    {
        await BaseOnParametersSetAsync();
        await OnWidgetParametersSetAsync();
        await BaseOnBeforeRenderAsync();
        await OnWidgetBeforeRenderAsync();
    }
    protected virtual void BaseOnParametersSet() { }
    protected virtual Task BaseOnParametersSetAsync() => Task.CompletedTask;
    protected virtual void OnWidgetParametersSet() { }
    protected virtual Task OnWidgetParametersSetAsync() => Task.CompletedTask;

    protected virtual void BaseOnBeforeRender() { }
    protected virtual Task BaseOnBeforeRenderAsync() => Task.CompletedTask;
    protected virtual void OnWidgetBeforeRender() { }
    protected virtual Task OnWidgetBeforeRenderAsync() => Task.CompletedTask;

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
        if (!_disposed)
        {
            await _disposeLock.WaitAsync(); // Ensure only one disposal operation happens at a time.
            try
            {
                if (!_disposed)
                {

                    await WidgetComponentLifecycle.KillComponent();
                    BaseOnDispose();
                    await BaseOnDisposeAsync();
                    OnWidgetDispose();
                    await OnWidgetDisposeAsync();
                    // Mark the object as disposed.
                    _disposed = true;
                }
            }
            finally
            {
                _disposeLock.Release();
            }
        }
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
    protected sealed override void BaseOnBeforeRender()
    {
        if (_widgetLifecycle != default)
            _widgetLifecycle.OnBeforeRender();
    }
    protected sealed override async Task BaseOnBeforeRenderAsync()
    {
        if (_widgetLifecycle != default)
            await _widgetLifecycle.OnBeforeRenderAsync();
    }
    protected sealed override void BaseOnDispose()
    {
        if (ViewModel is not null)
            ViewModel.SpreadChanges -= InvokeStateChanges;
    }
    protected sealed override Task BaseOnDisposeAsync() => base.BaseOnDisposeAsync();


}
