using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States.Models;
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
        var rollbacks = await _medihater.Send(new GetRollbackPackagesQuery(), dispatcher.CancelToken);
        IEnumerable<PackageLocalPulseModel> result = packages.Select(p =>
        {
            return new PackageLocalPulseModel()
            {
                Package = p,
                Rollback = rollbacks.SingleOrDefault(p => p.Info.PackageId == p.Info.PackageId) ?? default
            };
        });

        await dispatcher.Prepare<LoadLocalPackagesDoneAction>()
            .With(p => p.Packages, result)
            .DispatchAsync(dispatcher.CancelToken);
    }
}
