using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class SaveSourcesEffect : IEffect<SaveSourcesAction>
{
    private readonly IMedihater _medihater;

    public SaveSourcesEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(SaveSourcesAction action, IDispatcher dispatcher)
    {
        try
        {
            await _medihater.Send(new SaveSourcesCommand(action.Sources), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
        }
        catch (HostCodedException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
        finally
        {
            await dispatcher.Prepare<SaveSourcesDoneAction>().DispatchAsync();
        }
    }
}
