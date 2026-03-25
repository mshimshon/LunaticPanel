using LunaticPanel.Core.Abstraction.Tools;

namespace LunaticPanel.Engine.Web.Services.PanelControl;

public class PanelControl : IPanelControl
{
    public Func<Task>? DashboardStateHasChanged { get; set; }
    public Func<Task>? MenuStateHasChanged { get; set; }
    public Func<Task>? LayoutStateHasChanged { get; set; }

    public Guid Id { get; } = Guid.NewGuid();
    public async Task DashboardRender()
    {
        Console.WriteLine($"{Id} PanelControl: Dashboard Render");
        if (DashboardStateHasChanged != default)
        {
            Console.WriteLine("PanelControl: Dashboard DashboardStateHasChanged is SET");
            await DashboardStateHasChanged.Invoke();
        }
    }

    public async Task MenuRender()
    {
        if (MenuStateHasChanged != default)
            await MenuStateHasChanged.Invoke();
    }

    public async Task LayoutRender()
    {
        if (LayoutStateHasChanged != default)
            await LayoutStateHasChanged.Invoke();
    }
}
