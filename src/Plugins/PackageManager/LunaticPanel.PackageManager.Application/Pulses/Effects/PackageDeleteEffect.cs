using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class PackageDeleteEffect : IEffect<PackageDeleteAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<PackageDeleteEffect> _crazyReport;

    public PackageDeleteEffect(IMedihater medihater, ICrazyReport<PackageDeleteEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _medihater = medihater;
    }

    public async Task EffectAsync(PackageDeleteAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _medihater.Send(new PackageDeleteCommand(), dispatcher.CancelToken);
            await dispatcher.Prepare<PackageDeleteDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<PackageDeleteDoneAction>().DispatchAsync();
            throw;
        }
    }
}
