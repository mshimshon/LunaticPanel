using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class LoadSourcesEffect : IEffect<LoadSourcesAction>
{
    private readonly IMediator _mediator;

    public LoadSourcesEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task EffectAsync(LoadSourcesAction action, IDispatcher dispatcher)
    {
        ICollection<RepositorySourcePayload>? result = default;
        try
        {
            result = await _mediator.ExecuteAsync(new GetSourcesQuery(), dispatcher.CancelToken);
        }
        catch (HostCodedException)
        {
            throw;
        }
        catch (Exception ex)
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
