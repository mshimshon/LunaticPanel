using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class EnableSourceEffect : IEffect<EnableSourceAction>
{
    private readonly IMedihater _medihater;

    public EnableSourceEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }

    public async Task EffectAsync(EnableSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            await _medihater.Send(new EnableSourceCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<EnableSourceDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<EnableSourceDoneAction>().DispatchAsync();
            throw;
        }
    }
}
