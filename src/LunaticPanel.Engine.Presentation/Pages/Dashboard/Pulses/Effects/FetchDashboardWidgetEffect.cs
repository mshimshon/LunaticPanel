using LunaticPanel.Engine.Application.UI.Home.CQRS.Queries;
using LunaticPanel.Engine.Presentation.Pages.Dashboard.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation.Pages.Dashboard.Pulses.Effects;

public class FetchDashboardWidgetEffect : IEffect<FetchDashboardWidgetAction>
{
    private readonly IMedihater _medihater;

    public FetchDashboardWidgetEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(FetchDashboardWidgetAction action, IDispatcher dispatcher)
    {
        var result = await _medihater.Send(new FetchDashboardWidgetsQuery());
        await dispatcher.Prepare<FetchedDashboardWidgetAction>()
            .With(p => p.Widgets, result)
            .DispatchAsync();
    }
}
