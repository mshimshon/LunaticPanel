
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class MoveUpSourceEffect : IEffect<MoveUpSourceAction>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<MoveUpSourceEffect> _crazyReport;

    public MoveUpSourceEffect(IMediator mediator, ICrazyReport<MoveUpSourceEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
    }
    public async Task EffectAsync(MoveUpSourceAction action, IDispatcher dispatcher)
    {

        try
        {
            _crazyReport.Report("Executing Effect");

            await _mediator.ExecuteAsync(new MoveUpSourceCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<MoveUpSourceDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<MoveUpSourceDoneAction>().DispatchAsync();
            throw new HostUnkownException();
        }

    }
}

