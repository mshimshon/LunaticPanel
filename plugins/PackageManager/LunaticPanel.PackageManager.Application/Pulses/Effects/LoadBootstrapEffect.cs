using LunaticPanel.PackageManager.Application.Pulses.Actions;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class LoadBootstrapEffect : IEffect<LoadBootstrapAction>
{
    public Task EffectAsync(LoadBootstrapAction action, IDispatcher dispatcher)
        => throw new NotImplementedException();
}
