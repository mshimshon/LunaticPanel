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
    public IEnumerable<PackageManifestPayload> SearchResult { get; private set; } = new List<PackageManifestPayload>();
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
        var finalResult = new List<PackageManifestPayload>();
        foreach (var sourceResult in SearchPackageState.Search)
        {
            Console.WriteLine(sourceResult.Value.Result.Count());
            if (sourceResult.Value.TotalResults > 0 && sourceResult.Value.Position <= sourceResult.Value.TotalResults)
                foreach (var sourceSearch in sourceResult.Value.Result)
                    if (!finalResult.Any(p => p.Id == sourceSearch.Id))
                        finalResult.Add(sourceSearch);
        }
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
