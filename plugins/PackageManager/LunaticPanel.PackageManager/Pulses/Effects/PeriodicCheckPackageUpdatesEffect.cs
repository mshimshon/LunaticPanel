
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

    public PeriodicCheckPackageUpdatesEffect(IMediator mediator, ICrazyReport<PeriodicCheckPackageUpdatesEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }
    public async Task EffectAsync(PeriodicCheckPackageUpdatesAction action, IDispatcher dispatcher)
    {

        try
        {
            _crazyReport.Report("Executing Effect");
            var panelVersion = dispatcher.GetState<PanelUpdateState>().DownloadedVersion;
            var missingPlugins = dispatcher.GetState<PanelUpdateState>().MissingPlugins.ToList();
            if (panelVersion != default && missingPlugins.Count > 0)
            {
                var panelUpdate = await _mediator.ExecuteAsync(new GetPanelUpdateCompatiblePluginsQuery(missingPlugins, panelVersion), dispatcher.CancelToken);

            }
            else
            {
                var installed = dispatcher.GetState<PackageManagerDiskState>().Installed.ToDictionary(p => p.Info.PackageId, p => NuGetVersion.Parse(p.Version));
                // dispatcher.GetState<PanelUpdateState>().
                ICollection<PackageManifestPayload> result = await _mediator.ExecuteAsync(new PeriodicCheckPackageUpdatesQuery(installed), dispatcher.CancelToken);
                await dispatcher.Prepare<PeriodicCheckPackageUpdatesDoneAction>()
                    .With(p => p.AvailableUpdates, result)
                    .DispatchAsync();
            }
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<PeriodicCheckPackageUpdatesDoneAction>().DispatchAsync();
            throw new HostUnkownException();
        }

    }


}
