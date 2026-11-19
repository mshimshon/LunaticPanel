using LunaticPanel.Engine.Pages.Dashboard.Pulses.Actions;
using LunaticPanel.Engine.Pages.Dashboard.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Pages.Dashboard.Pulses.Reducers;

public class FetchDashboardWidgetReducer : IReducer<DashboardState, FetchDashboardWidgetAction>
{
    public DashboardState Reduce(DashboardState state, FetchDashboardWidgetAction action)
        => state with { IsLoading = true };
}
