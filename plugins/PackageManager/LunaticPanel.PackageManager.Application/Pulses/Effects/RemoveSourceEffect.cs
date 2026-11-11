using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class RemoveSourceEffect : IEffect<RemoveSourceAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<RemoveSourceEffect> _crazyReport;

    public RemoveSourceEffect(IMediator mediator, ICrazyReport<RemoveSourceEffect> crazyReport)
    {
        _mediator = mediator;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }
    public async Task EffectAsync(RemoveSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Removing {0}", action.Source.Source);
            await _mediator.ExecuteAsync(new RepositorySourceRemoveCommand(action.Source), dispatcher.CancelToken);
            _crazyReport.Report("{0} was removed", action.Source.Source);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<RemoveSourceDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<RemoveSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
