namespace LunaticPanel.Core.Abstraction.Tools;

public interface IPanelControl
{
    Version PanelVersion { get; }
    Task DashboardRender();
    Task MenuRender();
    Task Shutdown();
    Task LayoutRender();
    Guid Id { get; }
}
