
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class AutoUpdatePackagesEffect : IEffect<AutoUpdatePackagesAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<AutoUpdatePackagesEffect> _crazyReport;

    public AutoUpdatePackagesEffect(IMediator mediator, ICrazyReport<AutoUpdatePackagesEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }

    public async Task EffectAsync(AutoUpdatePackagesAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _mediator.ExecuteAsync(new AutoUpdatePackagesCommand(), dispatcher.CancelToken);
            await dispatcher.Prepare<AutoUpdatePackagesDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<AutoUpdatePackagesDoneAction>().DispatchAsync();
            throw;
        }
    }
}
