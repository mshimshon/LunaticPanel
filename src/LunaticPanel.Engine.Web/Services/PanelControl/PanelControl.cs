using LunaticPanel.Core.Abstraction.Tools;

namespace LunaticPanel.Engine.Web.Services.PanelControl;

public class PanelControl : IPanelControl
{
    public Func<Task>? DashboardStateHasChanged { get; set; }
    public Func<Task>? MenuStateHasChanged { get; set; }
    public Func<Task>? LayoutStateHasChanged { get; set; }

    public async Task DashboardRender()
    {
        if (DashboardStateHasChanged != default)
            await DashboardStateHasChanged.Invoke();
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
