using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class InstallNextUpdateEffect : IEffect<InstallNextUpdateAction>
{
    private readonly IMedihater _medihater;

    public InstallNextUpdateEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(InstallNextUpdateAction action, IDispatcher dispatcher)
    {
        try
        {
            await _medihater.Send(new PackageUpdateCommand(action.Package), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageRollbackAction>().Await().DispatchAsync();
            await dispatcher.Prepare<InstallNextUpdateDoneAction>()
                .With(p => p.ToRemove, action.Package)
                .DispatchAsync();

        }
        catch (Exception)
        {
            await dispatcher.Prepare<InstallNextUpdateDoneAction>().DispatchAsync();
            throw;
        }

    }
}
