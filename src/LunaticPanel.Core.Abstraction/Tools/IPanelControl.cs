namespace LunaticPanel.Core.Abstraction.Tools;

public interface IPanelControl
{
    Task DashboardRender();
    Task MenuRender();
    Task Shutdown();
    Task LayoutRender();
    Guid Id { get; }
}
