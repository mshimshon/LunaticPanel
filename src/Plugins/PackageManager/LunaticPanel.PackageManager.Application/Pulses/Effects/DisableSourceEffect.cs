
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class DisableSourceEffect : IEffect<DisableSourceAction>
{
    private readonly IMedihater _medihater;

    public DisableSourceEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(DisableSourceAction action, IDispatcher dispatcher)
    {

        try
        {
            await _medihater.Send(new DisableSourceCommand(), dispatcher.CancelToken);
            await dispatcher.Prepare<DisableSourceDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<DisableSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
