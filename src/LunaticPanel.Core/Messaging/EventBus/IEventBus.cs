using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EventBus;

public interface IEventBus
{
    IReadOnlyCollection<string> GetAvailableKeys();
    IReadOnlyCollection<Type> GetAllHandlersByEventId(MessageKey messageKey);
    Task PublishAsync(IEventBusMessage evt);
    bool HasKeyFor(MessageKey messageKey);
    bool HasKeyFor(string key);

}
