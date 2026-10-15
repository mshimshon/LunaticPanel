using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class EnablePluginEffect : IEffect<EnablePluginAction>
{
    private readonly IEventBus _eventBus;
    private readonly ICrazyReport<EnablePluginEffect> _crazyReport;

    public EnablePluginEffect(IEventBus eventBus, ICrazyReport<EnablePluginEffect> crazyReport)
    {
        _eventBus = eventBus;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task EffectAsync(EnablePluginAction action, IDispatcher dispatcher)
    {
        await _eventBus.PublishDataAsync(PluginKeys.Events.EnablePlugin, action.Id);
    }
}
