
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class PeriodicCheckPackageUpdatesEffect : IEffect<PeriodicCheckPackageUpdatesAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<PeriodicCheckPackageUpdatesEffect> _crazyReport;

    public PeriodicCheckPackageUpdatesEffect(IMedihater medihater, ICrazyReport<PeriodicCheckPackageUpdatesEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _medihater = medihater;
    }
    public async Task EffectAsync(PeriodicCheckPackageUpdatesAction action, IDispatcher dispatcher)
    {

        try
        {
            _crazyReport.Report("Executing Effect");

            ICollection<PackagePayload> result = await _medihater.Send(new PeriodicCheckPackageUpdatesQuery(), dispatcher.CancelToken);
            await dispatcher.Prepare<PeriodicCheckPackageUpdatesDoneAction>()
                .With(p => p.AvailableUpdates, result)
                .DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<PeriodicCheckPackageUpdatesDoneAction>().DispatchAsync();
            throw;
        }

    }
}
