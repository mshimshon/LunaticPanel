using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class SourceManagerCardViewModel : WidgetViewModelBase, ISourceManagerCardViewModel
{
    public RepositorySourcePayload Item { get; set; } = default!;
    private readonly IStateAccessor<RepositorySourceState> _sourceStateAccess;
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

    public SourceManagerCardViewModel(IStateAccessor<RepositorySourceState> sourceStateAccess)
    {
        _sourceStateAccess = sourceStateAccess;
    }

    public async Task MoveUp()
    {

    }

    public async Task MoveDown()
    {

    }
}
