using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Web.Services;

namespace LunaticPanel.Engine.Web.Hooks.Events;

[EventBusKey(PluginKeys.Events.DisablePlugin, ServiceLifetime = EBusLifetime.Scoped)]
internal class DisablePluginEvent : IEventBusHandler
{
    private readonly IBootstrapFileController _bootstrapFileController;
    private readonly IEventBus _eventBus;

    public DisablePluginEvent(IBootstrapFileController bootstrapFileController, IEventBus eventBus)
    {
        _bootstrapFileController = bootstrapFileController;
        _eventBus = eventBus;
    }
    public async Task HandleAsync(IEventBusMessage evt)
    {
        string id = await evt.ReadAs<string>();
        await _bootstrapFileController.DisablePlugin(id);
        await _eventBus.PublishDataAsync(PluginKeys.Events.OnPluginDisabled, id);
    }
}
