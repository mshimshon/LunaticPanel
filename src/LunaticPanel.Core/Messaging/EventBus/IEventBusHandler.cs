namespace LunaticPanel.Core.Messaging.EventBus;

public interface IEventBusHandler
{
    Task HandleAsync(IEventBusMessage evt);
}
