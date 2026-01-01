using LunaticPanel.Engine.Presentation.Pages.Dashboard.Pulses.Actions;
using LunaticPanel.Engine.Presentation.Pages.Dashboard.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation.Pages.Dashboard.Pulses.Reducers;

public class FetchedDashboardWidgetReducer : IReducer<DashboardState, FetchedDashboardWidgetAction>
{
    public Task<DashboardState> ReduceAsync(DashboardState state, FetchedDashboardWidgetAction action)
        => Task.FromResult(state with
        {
            IsLoading = false,
            Widgets = action.Widgets?.AsReadOnly() ?? default
        });
}
