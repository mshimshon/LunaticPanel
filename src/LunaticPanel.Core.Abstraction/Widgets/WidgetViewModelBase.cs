using LunaticPanel.Core.Abstraction.Widgets.Enum;

namespace LunaticPanel.Core.Abstraction.Widgets;


public abstract class WidgetViewModelBase : IWidgetViewModel, IWidgetLifecycleViewModel, IDisposable

{
    private bool _isLoading = false;
    private bool _disposedValue;

    public bool IsLoading
    {
        get => _isLoading || GetStateLoadingStatus();
        protected set
        {
            bool hasChanged = value != _isLoading;
            _isLoading = value;
            if (hasChanged)
                _ = UpdateChanges();
        }
    }

    public bool FirstRenderCompleted { get; set; }

    public event Func<SpreadChangeOption, Task>? SpreadChanges;

    /// <summary>
    /// Invokes the registered change propagation delegate, if any, to update changes and cause the parent component to rerender.
    /// </summary>
    protected Task UpdateChanges()
    {
        SpreadChanges?.Invoke(SpreadChangeOption.TouchMyComponentOnly);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Try to Notify the parent component of changes (if parent is Type Component and supported but the requestor).
    /// </summary>
    protected Task UpdateParentChanges()
    {
        SpreadChanges?.Invoke(SpreadChangeOption.TouchParentWhenPossible);
        return Task.CompletedTask;
    }

    /// <summary>
    /// IsLoading check the state of loading for the view model alone. <br />
    /// Override this method to add IsLoading = true under other conditions such as a IsLoading of a subscribed state.
    /// </summary>
    /// <returns></returns>
    protected virtual bool GetStateLoadingStatus() => false;
    protected virtual void OnViewModelDispose() { }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                OnViewModelDispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    public void OnInitialized() => OnViewModelInitialized();
    public Task OnInitializedAsync() => OnViewModelInitializedAsync();
    public void OnParametersSet() => OnViewModelParametersSet();
    public Task OnParametersSetAsync() => OnViewModelParametersSetAsync();
    public Task OnAfterRenderAsync(bool firstRender) => OnViewModelAfterRenderAsync(firstRender);
    public void OnAfterRender(bool firstRender)
    {
        OnViewModelAfterRender(firstRender);
        if (firstRender)
            FirstRenderCompleted = true;
    }

    protected virtual void OnViewModelInitialized() { }
    protected virtual Task OnViewModelInitializedAsync() => Task.CompletedTask;
    protected virtual void OnViewModelParametersSet() { }
    protected virtual Task OnViewModelParametersSetAsync() => Task.CompletedTask;
    protected virtual Task OnViewModelAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void OnViewModelAfterRender(bool firstRender) { }
}
