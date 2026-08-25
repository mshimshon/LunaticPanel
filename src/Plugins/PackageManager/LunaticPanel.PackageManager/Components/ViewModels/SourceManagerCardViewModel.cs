using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class SourceManagerCardViewModel : WidgetViewModelBase, ISourceManagerCardViewModel
{
    public RepositorySourcePayload Item { get; set; } = default!;
    private readonly IStateAccessor<RepositorySourceState> _sourceStateAccess;
    private readonly IDispatcher _dispatcher;

    public RepositorySourceState SourceState => _sourceStateAccess.State;
    public bool IsFirst()
    {
        var first = SourceState.Sources?.FirstOrDefault();
        if (first == default) return true;
        return first == Item;
    }
    protected override bool GetStateLoadingStatus() => SourceState.SourcesLoading || SourceState.SourceSaving;
    public bool IsLast()
    {
        var last = SourceState.Sources?.Last();
        if (last == default) return true;
        return last == Item;
    }

    public SourceManagerCardViewModel(IStateAccessor<RepositorySourceState> sourceStateAccess, IDispatcher dispatcher)
    {
        _sourceStateAccess = sourceStateAccess;
        _dispatcher = dispatcher;
    }

    public async Task MoveUp()
    {
        if (IsFirst()) return;
        IsLoading = true;
        var arr = SourceState.Sources.ToList();
        int index = arr.IndexOf(Item);
        int swapIndex = index - 1;
        var swap = arr[swapIndex];
        arr[swapIndex] = Item;
        arr[index] = swap;
        await _dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, arr)
            .DispatchAsync();
        IsLoading = false;
    }

    public async Task MoveDown()
    {
        if (IsLast()) return;
        IsLoading = true;
        var arr = SourceState.Sources.ToList();
        int index = arr.IndexOf(Item);
        int swapIndex = index + 1;
        var swap = arr[swapIndex];
        arr[swapIndex] = Item;
        arr[index] = swap;
        await _dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, arr)
            .DispatchAsync();
        IsLoading = false;
    }

    public async Task Delete()
    {
        IsLoading = true;
        var newList = SourceState.Sources.Where(p => p != Item).ToList();
        await _dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, newList)
            .DispatchAsync();
        IsLoading = false;
    }
}
