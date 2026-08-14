using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageSearchViewModel : WidgetViewModelBase, IPackageSearchViewModel
{
    private readonly IStatePulse _statePulse;
    private string _keywords = string.Empty;

    public SearchPackageState SearchPackageState => _statePulse.StateOf<SearchPackageState>(() => this, UpdateChanges);
    public IEnumerable<PackageInfoPayload> SearchResult { get; private set; } = new List<PackageInfoPayload>();
    public string Keywords
    {
        get => _keywords;
        set
        {
            _keywords = value;
            _ = SearchAsync();
        }
    }

    public PackageSearchViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }

    protected override void OnViewModelBeforeRender()
    {
        FilterResult();
    }
    private void FilterResult()
    {
        if (SearchPackageState.IsLoading) return;
        var finalResult = new List<PackageInfoPayload>();
        foreach (var sourceResult in SearchPackageState.Search)
            if (sourceResult.Value.Total > 0 && sourceResult.Value.Position <= sourceResult.Value.Total)
                foreach (var sourceSearch in sourceResult.Value.Result)
                    if (!finalResult.Any(p => p.PackageId == sourceSearch.PackageId))
                        finalResult.Add(sourceSearch);
        SearchResult = finalResult;
    }
    public async Task OnSearchAsync()
    {
        await _statePulse.Dispatcher.Prepare<SearchRemotePackageAction>()
             .With(p => p.Keywords, Keywords)
             .Await()
             .DispatchAsync();

    }
    public async Task SearchAsync() => await FailSafeExecutionAsync(OnSearchAsync);
}
