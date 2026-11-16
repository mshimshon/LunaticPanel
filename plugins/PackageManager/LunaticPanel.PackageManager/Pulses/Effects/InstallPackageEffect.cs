using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class InstallPackageEffect : IEffect<InstallPackageAction>
{
    private readonly IMediator _mediator;

    public InstallPackageEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task EffectAsync(InstallPackageAction action, IDispatcher dispatcher)
    {
        try
        {
            await _mediator.ExecuteAsync(new PackageInstallCommand(action.Target), dispatcher.CancelToken);
            await dispatcher.Prepare<LoadPackageDiskFoldersAction>().Await().DispatchAsync();
            await dispatcher.Prepare<InstallPackageDoneAction>().DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<InstallPackageDoneAction>().DispatchAsync();
            throw new HostUnkownException();
        }

    }
}
