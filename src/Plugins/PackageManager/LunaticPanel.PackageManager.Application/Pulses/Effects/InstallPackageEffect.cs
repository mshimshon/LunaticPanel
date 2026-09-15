using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class InstallPackageEffect : IEffect<InstallPackageAction>
{
    private readonly IMedihater _medihater;

    public InstallPackageEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(InstallPackageAction action, IDispatcher dispatcher)
    {
        try
        {
            await _medihater.Send(new PackageInstallCommand(action.Target, action.Source), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageRollbackAction>().Await().DispatchAsync();
            await dispatcher.Prepare<InstallPackageDoneAction>().DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<InstallPackageDoneAction>().DispatchAsync();
            throw;
        }

    }
}
