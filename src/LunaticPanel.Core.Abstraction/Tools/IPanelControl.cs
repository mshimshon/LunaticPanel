using NuGet.Versioning;

namespace LunaticPanel.Core.Abstraction.Tools;

public interface IPanelControl
{
    string PanelVersion { get; }
    Task DashboardRender();
    Task MenuRender();
    Task Shutdown();
    Task LayoutRender();
    Guid Id { get; }
}
