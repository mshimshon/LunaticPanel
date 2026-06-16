using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

public sealed class LoadLocalPackagesEffect : IEffect<LoadLocalPackagesAction>
{
    private readonly IMedihater _medihater;

    public LoadLocalPackagesEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }

    public async Task EffectAsync(LoadLocalPackagesAction action, IDispatcher dispatcher)
    {
        var packages = await _medihater.Send(new GetAllPackagesQuery(), dispatcher.CancelToken);
        await dispatcher.Prepare<LoadLocalPackagesDoneAction>()
            .With(p => p.Packages, packages)
            .DispatchAsync(dispatcher.CancelToken);

    }
}
