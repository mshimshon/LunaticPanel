using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class LoadPackageDiskFoldersEffect : IEffect<LoadPackageDiskFoldersAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<LoadPackageDiskFoldersAction> _crazyReport;

    public LoadPackageDiskFoldersEffect(IMediator mediator, ICrazyReport<LoadPackageDiskFoldersAction> crazyReport)
    {
        _mediator = mediator;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }

    public async Task EffectAsync(LoadPackageDiskFoldersAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _mediator.ExecuteAsync(new GetDiskPackagesQuery(), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageDiskFoldersDoneAction>()
                .With(p => p.AvailableRollbacks, result.AvailableRollbacks)
                .With(p => p.Installed, result.Installed)
                .With(p => p.PendingDelete, result.PendingDelete)
                .With(p => p.PendingUpdates, result.PendingUpdates)
                .With(p => p.PreInstalled, result.PreInstalled)
                .With(p => p.RuntimePackages, result.RuntimePackages)
                .DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<LoadPackageDiskFoldersDoneAction>().DispatchAsync();
            throw;
        }

    }
}
