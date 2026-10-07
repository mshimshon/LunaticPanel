using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface ISourceManagerCardViewModel : IWidgetViewModel
{
    RepositorySourcePayload Item { get; set; }
    RepositorySourceState SourceState { get; }
    string[] AvailableApiVersion { get; }
    bool IsLast();
    bool IsFirst();
    Task MoveUp();
    Task MoveDown();
    Task Delete();
    Task Test();
    Task Enable();
    Task Disable();
}
