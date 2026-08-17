using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class SourceManagerViewModel : WidgetViewModelBase, ISourceManagerViewModel
{
    private readonly IStatePulse _statePulse;
    public RepositorySourceState SourceState => _statePulse.StateOf<RepositorySourceState>(() => this, UpdateChanges);
    public SourceManagerViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }
}
