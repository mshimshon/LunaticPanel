using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class LoadSourcesEffect : IEffect<LoadSourcesAction>
{
    private readonly IMedihater _medihater;

    public LoadSourcesEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(LoadSourcesAction action, IDispatcher dispatcher)
    {
        ICollection<Payloads.RepositorySourcePayload>? result = default;
        try
        {
            result = await _medihater.Send(new GetRepositorySourcesQuery(), dispatcher.CancelToken);
        }
        catch (HostCodedException)
        { throw; }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
        finally
        {
            await dispatcher.Prepare<LoadSourcesDoneAction>()
            .With(p => p.Sources, result)
            .DispatchAsync();
        }
    }
}
