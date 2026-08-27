using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Infrastructure.Services;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class SourceManagerCardViewModel : WidgetViewModelBase, ISourceManagerCardViewModel
{
    public RepositorySourcePayload Item { get; set; } = default!;
    private readonly IStateAccessor<RepositorySourceState> _sourceStateAccess;
    private readonly IDispatcher _dispatcher;
    private readonly IExternalSourceService _externalSourceService;
    public string[] AvailableApiVersion { get; set; } = default!;
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

    public SourceManagerCardViewModel(IStateAccessor<RepositorySourceState> sourceStateAccess, IDispatcher dispatcher, IExternalSourceService externalSourceService)
    {
        _sourceStateAccess = sourceStateAccess;
        _dispatcher = dispatcher;
        _externalSourceService = externalSourceService;
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
    protected override async Task OnViewModelAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Item.SourceType == Application.Payloads.Enums.RepositorySourceTypePayload.Remote)
                AvailableApiVersion = await _externalSourceService.GetAPIVersionsAsync(Item);
        }
    }
    public async Task Test()
    {
        if (Item.SourceType == Application.Payloads.Enums.RepositorySourceTypePayload.Remote)
            await TestRemote();
        else
            await TestLocal();
    }

    private async Task TestRemote()
    {
        IsLoading = true;
        var source = SourceState.Sources.ToList();
        int index = source.IndexOf(Item);
        AvailableApiVersion = await _externalSourceService.GetAPIVersionsAsync(Item);
        await _dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, source)
            .DispatchAsync();
        IsLoading = false;
    }
    private async Task TestLocal()
    {
        IsLoading = true;
        var source = SourceState.Sources.ToList();
        int index = source.IndexOf(Item);
        if (!Directory.Exists(Item.Source))
            source[index] = source[index] with { Failure = "Location not found on system." };
        if (Directory.GetFiles(Item.Source, "*.lpkg", SearchOption.AllDirectories).Length <= 0)
            source[index] = source[index] with { Failure = "Location doesn't contain any lpkgs" };
        await _dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, source)
            .DispatchAsync();
        IsLoading = false;
    }

    public async Task Enable()
    {
        if (Item.State == Application.Payloads.Enums.RepositorySourceStatePayload.Enabled) return;
        IsLoading = true;
        var arr = SourceState.Sources.ToList();
        int index = arr.IndexOf(Item);
        arr[index] = Item with { State = Application.Payloads.Enums.RepositorySourceStatePayload.Enabled };
        await _dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, arr)
            .DispatchAsync();
        IsLoading = false;
    }
    public async Task Disable()
    {
        if (Item.State == Application.Payloads.Enums.RepositorySourceStatePayload.Disabled) return;
        IsLoading = true;
        var arr = SourceState.Sources.ToList();
        int index = arr.IndexOf(Item);
        arr[index] = Item with { State = Application.Payloads.Enums.RepositorySourceStatePayload.Disabled };
        await _dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, arr)
            .DispatchAsync();
        IsLoading = false;
    }
}
