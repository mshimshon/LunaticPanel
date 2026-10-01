
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class AutoUpdatePackagesEffect : IEffect<AutoUpdatePackagesAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<AutoUpdatePackagesEffect> _crazyReport;

    public AutoUpdatePackagesEffect(IMedihater medihater, ICrazyReport<AutoUpdatePackagesEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _medihater = medihater;
    }

    public async Task EffectAsync(AutoUpdatePackagesAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _medihater.Send(new AutoUpdatePackagesCommand(), dispatcher.CancelToken);
            await dispatcher.Prepare<AutoUpdatePackagesDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<AutoUpdatePackagesDoneAction>().DispatchAsync();
            throw;
        }
    }
}
