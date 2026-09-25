using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class MoveDownSourceEffect : IEffect<MoveDownSourceAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<MoveDownSourceEffect> _crazyReport;

    public MoveDownSourceEffect(IMedihater medihater, ICrazyReport<MoveDownSourceEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _medihater = medihater;
    }
    public async Task EffectAsync(MoveDownSourceAction action, IDispatcher dispatcher)
    {

        try
        {
            _crazyReport.Report("Executing Effect");

            await _medihater.Send(new MoveDownSourceCommand(action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<MoveDownSourceDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<MoveDownSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
