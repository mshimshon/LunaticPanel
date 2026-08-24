using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Services;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class SaveSourcesEffect : IEffect<SaveSourcesAction>
{
    private readonly ISourceService _sourceService;

    public SaveSourcesEffect(ISourceService sourceService)
    {
        _sourceService = sourceService;
    }
    public async Task EffectAsync(SaveSourcesAction action, IDispatcher dispatcher)
    {
        ICollection<RepositorySourcePayload>? result = default;
        try
        {
            result = await _sourceService.SaveSourcesAsync(action.Sources, dispatcher.CancelToken);
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
            await dispatcher.Prepare<SaveSourcesDoneAction>().With(p => p.Sources, result).DispatchAsync();
        }
    }
}
