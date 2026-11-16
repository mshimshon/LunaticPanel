using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class PackageDeleteEffect : IEffect<PackageDeleteAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<PackageDeleteEffect> _crazyReport;

    public PackageDeleteEffect(IMediator mediator, ICrazyReport<PackageDeleteEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }

    public async Task EffectAsync(PackageDeleteAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _mediator.ExecuteAsync(new PackageRemoveCommand(action.Package.Info.PackageId), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageDiskFoldersAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
        finally
        {
            await dispatcher.Prepare<PackageDeleteDoneAction>().DispatchAsync();
        }
    }
}
