using LunaticPanel.Engine.Layout.Menu.Pulses.Actions;
using LunaticPanel.Engine.Mediator.Queries;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.Engine.Layout.Menu.Pulses.Effects;

public class FetchMenuElementsEffect : IEffect<FetchMenuElementsAction>
{
    private readonly IMediator _mediator;

    public FetchMenuElementsEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task EffectAsync(FetchMenuElementsAction action, IDispatcher dispatcher)
    {
        var result = await _mediator.ExecuteAsync(new FetchMenuElementQuery());
        await dispatcher.Prepare<FetchedMenuElementsAction>().With(p => p.MenuElements, result).DispatchAsync();
    }
}
