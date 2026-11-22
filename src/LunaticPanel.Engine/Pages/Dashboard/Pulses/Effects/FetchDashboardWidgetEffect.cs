using LunaticPanel.Engine.Application.UI.Home.Mediator.Queries;
using LunaticPanel.Engine.Pages.Dashboard.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.Engine.Pages.Dashboard.Pulses.Effects;

public class FetchDashboardWidgetEffect : IEffect<FetchDashboardWidgetAction>
{
    private readonly IMediator _mediator;

    public FetchDashboardWidgetEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task EffectAsync(FetchDashboardWidgetAction action, IDispatcher dispatcher)
    {
        //await Task.Delay(1000); // TODO: MONITOR THIS
        var result = await _mediator.ExecuteAsync(new FetchDashboardWidgetsQuery());
        await dispatcher.Prepare<FetchedDashboardWidgetAction>()
            .With(p => p.Widgets, result)
            .DispatchAsync();
    }
}
