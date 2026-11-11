
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class DisableSourceEffect : IEffect<DisableSourceAction>
{
    private readonly IMediator _mediator;

    public DisableSourceEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task EffectAsync(DisableSourceAction action, IDispatcher dispatcher)
    {

        try
        {
            await _mediator.ExecuteAsync(new DisableSourceCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<DisableSourceDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<DisableSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
