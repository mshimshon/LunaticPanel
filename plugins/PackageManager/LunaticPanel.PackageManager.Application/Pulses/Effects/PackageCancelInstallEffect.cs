using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class PackageCancelInstallEffect : IEffect<PackageCancelInstallAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<PackageCancelInstallEffect> _crazyReport;

    public PackageCancelInstallEffect(IMedihater medihater, ICrazyReport<PackageCancelInstallEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _medihater = medihater;
    }

    public async Task EffectAsync(PackageCancelInstallAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _medihater.Send(new PackageCancelInstallCommand(action.Package), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageDiskFoldersAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<PackageCancelInstallDoneAction>().DispatchAsync();
            throw;
        }
    }
}
