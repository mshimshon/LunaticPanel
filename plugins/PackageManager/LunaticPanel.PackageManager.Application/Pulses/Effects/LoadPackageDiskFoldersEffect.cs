using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class LoadPackageDiskFoldersEffect : IEffect<LoadPackageDiskFoldersAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<LoadPackageDiskFoldersAction> _crazyReport;

    public LoadPackageDiskFoldersEffect(IMedihater medihater, ICrazyReport<LoadPackageDiskFoldersAction> crazyReport)
    {
        _medihater = medihater;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }

    public async Task EffectAsync(LoadPackageDiskFoldersAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _medihater.Send(new GetDiskPackagesQuery(), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageDiskFoldersDoneAction>()
                .With(p => p.AvailableRollbacks, result.AvailableRollbacks)
                .With(p => p.Installed, result.Installed)
                .With(p => p.PendingDelete, result.PendingDelete)
                .With(p => p.PendingUpdates, result.PendingUpdates)
                .With(p => p.PreInstalled, result.PreInstalled)
                .With(p => p.RuntimePackages, result.RuntimePackages)
                .DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<LoadPackageDiskFoldersDoneAction>().DispatchAsync();
            throw;
        }

    }
}
