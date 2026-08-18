using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface ISourceManagerViewModel : IWidgetViewModel
{
    RepositorySourceState SourceState { get; }
    Task LoadSources();
}
