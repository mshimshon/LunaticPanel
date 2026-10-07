using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class AddSourceEffect : IEffect<AddSourceAction>
{
    private readonly IMedihater _medihater;

    public AddSourceEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(AddSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            await _medihater.Send(new RepositorySourceAddCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<AddSourceDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<AddSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
