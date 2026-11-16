
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using LunaticPanel.PackageManager.Pulses.States;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using StatePulse.Net;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class PeriodicCheckPackageUpdatesEffect : IEffect<PeriodicCheckPackageUpdatesAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<PeriodicCheckPackageUpdatesEffect> _crazyReport;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;

    public PeriodicCheckPackageUpdatesEffect(IMediator mediator, ICrazyReport<PeriodicCheckPackageUpdatesEffect> crazyReport, IStateAccessor<PackageManagerDiskState> _packageManagerDiskState)
    {
        _crazyReport = crazyReport;
        this._packageManagerDiskState = _packageManagerDiskState;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }
    public async Task EffectAsync(PeriodicCheckPackageUpdatesAction action, IDispatcher dispatcher)
    {

        try
        {
            _crazyReport.Report("Executing Effect");
            var installed = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => NuGetVersion.Parse(p.Version));
            ICollection<PackageManifestPayload> result = await _mediator.ExecuteAsync(new PeriodicCheckPackageUpdatesQuery(installed), dispatcher.CancelToken);
            await dispatcher.Prepare<PeriodicCheckPackageUpdatesDoneAction>()
                .With(p => p.AvailableUpdates, result)
                .DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<PeriodicCheckPackageUpdatesDoneAction>().DispatchAsync();
            throw new HostUnkownException();
        }

    }
}
