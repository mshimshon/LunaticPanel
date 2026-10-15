using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Hooks.Events;

[EventBusKey(PluginKeys.Events.OnPluginEnabled)]
internal class OnPluginDisabledChanged : IEventBusHandler
{
    private readonly IDispatcher _dispatcher;

    public OnPluginDisabledChanged(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    public async Task HandleAsync(IEventBusMessage evt)
    {
        await _dispatcher.Prepare<LoadBootstrapAction>().DispatchAsync();
    }
}
