using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageSearchViewModel : WidgetViewModelBase, IPackageSearchViewModel
{
    private readonly IStatePulse _statePulse;
    private string _keywords = string.Empty;

    public SearchPackageState SearchPackageState => _statePulse.StateOf<SearchPackageState>(() => this, UpdateChanges);

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
    public async Task OnSearchAsync()
    {
        await _statePulse.Dispatcher.Prepare<SearchRemotePackageAction>()
             .With(p => p.Keywords, Keywords)
             .Await()
             .DispatchAsync();

    }
    public async Task SearchAsync() => await FailSafeExecutionAsync(OnSearchAsync);
}
