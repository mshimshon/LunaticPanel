using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Pulses.Actions;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Hooks.Events;

[EventBusKey(PluginKeys.Events.ON_PLUGIN_DISABLED)]
internal class OnPluginDisabledChanged : IEventBusHandler
{
    private readonly IDispatcher _dispatcher;

    public OnPluginDisabledChanged(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    public async Task HandleAsync(IEventBusMessage evt)
    {
        Console.WriteLine("OnPluginDisabledChanged");
        await _dispatcher.Prepare<LoadBootstrapAction>().DispatchAsync();
    }
}
