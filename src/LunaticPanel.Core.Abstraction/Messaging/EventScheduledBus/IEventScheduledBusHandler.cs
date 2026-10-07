namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public interface IEventScheduledBusHandler
{
    EventScheduledBusMessageData DueToExecute(IEventScheduledBusMessage msg, CancellationToken ct = default);
}