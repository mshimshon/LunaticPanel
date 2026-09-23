using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class RemoveSourceEffect : IEffect<RemoveSourceAction>
{
    private readonly IMedihater _medihater;

    public RemoveSourceEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(RemoveSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            await _medihater.Send(new RepositorySourceRemoveCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<RemoveSourceDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<RemoveSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
