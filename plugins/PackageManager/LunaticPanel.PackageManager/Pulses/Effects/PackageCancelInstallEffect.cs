using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class PackageCancelInstallEffect : IEffect<PackageCancelInstallAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<PackageCancelInstallEffect> _crazyReport;

    public PackageCancelInstallEffect(IMediator mediator, ICrazyReport<PackageCancelInstallEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }

    public async Task EffectAsync(PackageCancelInstallAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _mediator.ExecuteAsync(new PackageCancelInstallCommand(action.Package), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageDiskFoldersAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
        finally
        {
            await dispatcher.Prepare<PackageCancelInstallDoneAction>().DispatchAsync();
        }
    }
}
