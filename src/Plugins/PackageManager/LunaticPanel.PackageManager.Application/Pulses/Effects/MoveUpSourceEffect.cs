
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class MoveUpSourceEffect : IEffect<MoveUpSourceAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<MoveUpSourceEffect> _crazyReport;

    public MoveUpSourceEffect(IMedihater medihater, ICrazyReport<MoveUpSourceEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _medihater = medihater;
    }
    public async Task EffectAsync(MoveUpSourceAction action, IDispatcher dispatcher)
    {

        try
        {
            _crazyReport.Report("Executing Effect");

            await _medihater.Send(new MoveUpSourceCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<MoveUpSourceDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<MoveUpSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}

