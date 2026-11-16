using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using StatePulse.Net;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

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
        try
        {
            await _eventBus.PublishDataAsync(PluginKeys.Events.DISABLE_PLUGIN, action.Id);
        }
        catch (System.Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new HostUnkownException();
        }
    }
}
