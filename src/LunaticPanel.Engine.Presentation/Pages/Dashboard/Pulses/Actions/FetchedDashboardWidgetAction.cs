using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.Actions;

public class FetchedDashboardWidgetAction : ISafeAction
{
    public List<WidgetElementEntity>? Widgets { get; set; }
}
