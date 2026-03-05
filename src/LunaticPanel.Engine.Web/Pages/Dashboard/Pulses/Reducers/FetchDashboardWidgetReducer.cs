using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.Actions;
using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.Reducers;

public class FetchDashboardWidgetReducer : IReducer<DashboardState, FetchDashboardWidgetAction>
{
    public DashboardState Reduce(DashboardState state, FetchDashboardWidgetAction action)
        => state with { IsLoading = true };
}
