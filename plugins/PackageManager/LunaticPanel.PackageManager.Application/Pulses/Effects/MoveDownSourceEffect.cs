using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class MoveDownSourceEffect : IEffect<MoveDownSourceAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<MoveDownSourceEffect> _crazyReport;

    public MoveDownSourceEffect(IMediator mediator, ICrazyReport<MoveDownSourceEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }
    public async Task EffectAsync(MoveDownSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _mediator.ExecuteAsync(new MoveDownSourceCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<MoveDownSourceDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<MoveDownSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
