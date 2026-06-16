using LunaticPanel.PackageManager.Application.Pulses.Actions;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class LoadPackageRollbackEffect : IEffect<LoadPackageRollbackAction>
{
    public async Task EffectAsync(LoadPackageRollbackAction action, IDispatcher dispatcher)
    {

    }
}
