using LunaticPanel.Core.Abstraction.Messaging.EventBus;

namespace LunaticPanel.Core.Extensions;

public static class EventBusExt
{
    public static async Task PublishDatalessAsync(this IEventBus eventbus, string key)
        => await eventbus.PublishAsync(new EventBusMessage(new(key)));
    public static async Task PublishDataAsync(this IEventBus eventbus, string key, object data)
        => await eventbus.PublishAsync(new EventBusMessage(new(key), data));
}
