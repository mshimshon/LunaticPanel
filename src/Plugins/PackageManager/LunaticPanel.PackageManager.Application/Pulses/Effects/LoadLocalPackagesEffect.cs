using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using StatePulse.Net;
using Wolverine;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

public sealed class LoadLocalPackagesEffect : IEffect<LoadLocalPackagesAction>
{
    private readonly IMessageBus _messageBus;

    public LoadLocalPackagesEffect(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public Task EffectAsync(LoadLocalPackagesAction action, IDispatcher dispatcher)
    {
        SearchResponse<PackageInfoPayload> result = await _messageBus.SendAsync(new SearchPackageQuery());
    }
}
