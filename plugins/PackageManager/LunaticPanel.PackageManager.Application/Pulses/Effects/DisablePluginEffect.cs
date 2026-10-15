using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class DisablePluginEffect : IEffect<DisablePluginAction>
{
    private readonly IEventBus _eventBus;
    private readonly ICrazyReport<DisablePluginAction> _crazyReport;

    public DisablePluginEffect(IEventBus eventBus, ICrazyReport<DisablePluginAction> crazyReport)
    {
        _eventBus = eventBus;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task EffectAsync(DisablePluginAction action, IDispatcher dispatcher)
    {
        await _eventBus.PublishDataAsync(PluginKeys.Events.DisablePlugin, action.Id);
    }
}
