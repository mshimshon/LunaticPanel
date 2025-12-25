namespace LunaticPanel.Core.Messaging.EventBus;

public interface IEventBus
{
    IReadOnlyCollection<string> GetAllEventIds();
    IReadOnlyCollection<Type> GetAllHandlersByEventId(string eventId);
    Task PublishAsync(IEventBusMessage evt);
}
