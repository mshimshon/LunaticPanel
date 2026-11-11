using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class AddSourceEffect : IEffect<AddSourceAction>
{
    private readonly IMediator _mediator;

    public AddSourceEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task EffectAsync(AddSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            await _mediator.ExecuteAsync(new RepositorySourceAddCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<AddSourceDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<AddSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
