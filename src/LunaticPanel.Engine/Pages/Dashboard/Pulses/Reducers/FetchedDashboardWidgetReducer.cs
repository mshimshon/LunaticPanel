using LunaticPanel.Engine.Pages.Dashboard.Pulses.Actions;
using LunaticPanel.Engine.Pages.Dashboard.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Pages.Dashboard.Pulses.Reducers;

public class FetchedDashboardWidgetReducer : IReducer<DashboardState, FetchedDashboardWidgetAction>
{
    public DashboardState Reduce(DashboardState state, FetchedDashboardWidgetAction action)
        => state with
        {
            IsLoading = false,
            Widgets = action.Widgets?.AsReadOnly() ?? default
        };
}
