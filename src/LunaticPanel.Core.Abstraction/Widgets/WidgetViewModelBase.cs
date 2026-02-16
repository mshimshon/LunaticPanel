using LunaticPanel.Core.Abstraction.Widgets.Enum;

namespace LunaticPanel.Core.Abstraction.Widgets;


public abstract class WidgetViewModelBase : IWidgetViewModel, IAsyncDisposable

{
    private bool _isLoading = false;
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
    public async ValueTask DisposeAsync()
    {
        OnViewModelDispose();
        await OnViewModelDisposeAsync();
    }
    protected virtual void OnViewModelDispose() { }
    protected virtual Task OnViewModelDisposeAsync()
     => Task.CompletedTask;
}
