using LunaticPanel.Core.Utils.Abstraction.Logging;
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
            //await _medihater.Send(new PackageInstallCommand(action.Target, action.Source), dispatcher.CancelToken);
            //await dispatcher.Prepare<LoadPackageRollbackAction>().Await().DispatchAsync();
            await dispatcher.Prepare<LoadPackageDiskFoldersDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<LoadPackageDiskFoldersDoneAction>().DispatchAsync();

            throw;
        }

    }
}
