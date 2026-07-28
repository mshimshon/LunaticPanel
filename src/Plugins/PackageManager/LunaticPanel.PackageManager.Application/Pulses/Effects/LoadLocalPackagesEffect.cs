using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

public sealed class LoadLocalPackagesEffect : IEffect<LoadLocalPackagesAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<LoadLocalPackagesAction> _crazyReport;

    public LoadLocalPackagesEffect(IMedihater medihater, ICrazyReport<LoadLocalPackagesAction> crazyReport)
    {
        _medihater = medihater;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }

    public async Task EffectAsync(LoadLocalPackagesAction action, IDispatcher dispatcher)
    {
        _crazyReport.Report("Load Local Packages");
        var packages = await _medihater.Send(new GetAllPackagesQuery(), dispatcher.CancelToken);
        await dispatcher.Prepare<LoadLocalPackagesDoneAction>()
            .With(p => p.Packages, packages)
            .DispatchAsync(dispatcher.CancelToken);
        _crazyReport.Report("Done");

    }
}
