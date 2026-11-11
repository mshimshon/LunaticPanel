using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class EnableSourceEffect : IEffect<EnableSourceAction>
{
    private readonly IMediator _mediator;

    public EnableSourceEffect(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task EffectAsync(EnableSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            await _mediator.ExecuteAsync(new EnableSourceCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<EnableSourceDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<EnableSourceDoneAction>().DispatchAsync();
            throw;
        }
    }
}
