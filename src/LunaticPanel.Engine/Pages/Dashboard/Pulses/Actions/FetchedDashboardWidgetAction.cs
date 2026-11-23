using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Models.Dashboard;
using StatePulse.Net;

namespace LunaticPanel.Engine.Pages.Dashboard.Pulses.Actions;

public class FetchedDashboardWidgetAction : ISafeAction
{
    public List<EngineBusMsgResponseWithData<WidgetElementModel>>? Widgets { get; set; }
}
