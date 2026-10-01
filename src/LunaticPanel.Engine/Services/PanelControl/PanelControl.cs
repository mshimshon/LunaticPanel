using LunaticPanel.Core.Abstraction.Tools;
using System.Reflection;

namespace LunaticPanel.Engine.Web.Services.PanelControl;

public class PanelControl : IPanelControl, IHostControl
{
    public Func<Task>? DashboardStateHasChanged { get; set; }
    public Func<Task>? MenuStateHasChanged { get; set; }
    public Func<Task>? LayoutStateHasChanged { get; set; }

    public Guid Id { get; } = Guid.NewGuid();

    public Version PanelVersion { get; }
    public PanelControl()
    {
        PanelVersion = Assembly.GetExecutingAssembly().GetName().Version!;
    }
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

    public Task Shutdown()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }

    public Task RestartAsync() => throw new NotImplementedException();
}
