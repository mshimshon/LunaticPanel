using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Core.Utils.Abstraction.Logging;
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
    private readonly ICrazyReport<SourceManagerCardViewModel> _crazyReport;

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

    public SourceManagerCardViewModel(IStateAccessor<RepositorySourceState> sourceStateAccess,
        IDispatcher dispatcher,
        IExternalSourceService externalSourceService, ICrazyReport<SourceManagerCardViewModel> crazyReport)
    {
        _sourceStateAccess = sourceStateAccess;
        _dispatcher = dispatcher;
        _externalSourceService = externalSourceService;
        _crazyReport = crazyReport;
    }



    public async Task MoveUp()
    {
        if (IsFirst()) return;
        IsLoading = true;

        await _dispatcher.Prepare<MoveUpSourceAction>()
            .With(p => p.Source, Item)
            .DispatchAsync();
        IsLoading = false;
    }

    public async Task MoveDown()
    {
        if (IsLast()) return;
        IsLoading = true;
        await _dispatcher.Prepare<MoveDownSourceAction>()
            .With(p => p.Source, Item)
            .DispatchAsync();
        IsLoading = false;
    }

    public async Task Delete()
    {
        _crazyReport.Report("Delete Clicked");
        IsLoading = true;
        _crazyReport.Report("Delete Dispatching");
        await _dispatcher.Prepare<RemoveSourceAction>()
            .With(p => p.Source, Item)
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

        IsLoading = false;
    }

    public async Task Enable()
    {
        if (Item.State == Application.Payloads.Enums.RepositorySourceStatePayload.Enabled) return;
        IsLoading = true;
        await _dispatcher.Prepare<EnableSourceAction>()
            .With(p => p.Source, Item)
            .DispatchAsync();
        IsLoading = false;
    }
    public async Task Disable()
    {
        if (Item.State == Application.Payloads.Enums.RepositorySourceStatePayload.Disabled) return;
        IsLoading = true;
        await _dispatcher.Prepare<DisableSourceAction>()
            .With(p => p.Source, Item)
            .DispatchAsync();
        IsLoading = false;
    }
}
