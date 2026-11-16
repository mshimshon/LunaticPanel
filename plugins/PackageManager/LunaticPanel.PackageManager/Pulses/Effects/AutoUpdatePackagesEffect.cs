
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class AutoUpdatePackagesEffect : IEffect<AutoUpdatePackagesAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<AutoUpdatePackagesEffect> _crazyReport;
    private readonly IStateAccessor<PackageUpdateState> _packageUpdateState;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;

    public AutoUpdatePackagesEffect(
        IMediator mediator,
        ICrazyReport<AutoUpdatePackagesEffect> crazyReport,
        IStateAccessor<PackageUpdateState> packageUpdateState,
        IStateAccessor<PackageManagerDiskState> packageManagerDiskState)
    {
        _crazyReport = crazyReport;
        _packageUpdateState = packageUpdateState;
        _packageManagerDiskState = packageManagerDiskState;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }

    public async Task EffectAsync(AutoUpdatePackagesAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            var currentlyInstalledVersions = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => p.Version);
            var foundUpdates = _packageUpdateState.State.FoundUpdates.ToList();
            await _mediator.ExecuteAsync(new AutoUpdatePackagesCommand(foundUpdates, currentlyInstalledVersions), dispatcher.CancelToken);
            await dispatcher.Prepare<AutoUpdatePackagesDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<AutoUpdatePackagesDoneAction>().DispatchAsync();
            throw new HostUnkownException();
        }
    }
}
