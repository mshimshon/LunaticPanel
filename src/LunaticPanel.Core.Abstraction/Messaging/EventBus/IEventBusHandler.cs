namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public interface IEventBusHandler
{
    Task HandleAsync(IEventBusMessage evt);
}
