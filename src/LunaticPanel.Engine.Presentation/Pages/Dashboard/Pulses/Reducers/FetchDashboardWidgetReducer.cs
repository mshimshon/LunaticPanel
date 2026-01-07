using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.Actions;
using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.Reducers;

public class FetchDashboardWidgetReducer : IReducer<DashboardState, FetchDashboardWidgetAction>
{
    public Task<DashboardState> ReduceAsync(DashboardState state, FetchDashboardWidgetAction action)
        => Task.FromResult(state with { IsLoading = true });
}
