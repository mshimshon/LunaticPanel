using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class RemoveSourceEffect : IEffect<RemoveSourceAction>
{
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<RemoveSourceEffect> _crazyReport;

    public RemoveSourceEffect(IMedihater medihater, ICrazyReport<RemoveSourceEffect> crazyReport)
    {
        _medihater = medihater;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }
    public async Task EffectAsync(RemoveSourceAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Removing {0}", action.Source.Source);
            await _medihater.Send(new RepositorySourceRemoveCommand(action.Source), dispatcher.CancelToken);
            _crazyReport.Report("{0} was removed", action.Source.Source);
            await dispatcher.Prepare<LoadSourcesAction>().Await().DispatchAsync();
            await dispatcher.Prepare<RemoveSourceDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<RemoveSourceDoneAction>().DispatchAsync();
            throw;
        }

    }
}
