using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Services;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class LoadSourcesEffect : IEffect<LoadSourcesAction>
{
    private readonly ISourceService _sourceService;

    public LoadSourcesEffect(ISourceService sourceService)
    {
        _sourceService = sourceService;
    }
    public async Task EffectAsync(LoadSourcesAction action, IDispatcher dispatcher)
    {
        ICollection<Payloads.RepositorySourcePayload>? result = default;
        try
        {
            result = await _sourceService.GetSourcesAsync(dispatcher.CancelToken);

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
