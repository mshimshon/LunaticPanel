namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public interface IEventScheduledBusExchange
{
    IReadOnlyCollection<string> GetAvailableKeys();
    Task<EventScheduledBusMessageResponse> ExchangeAsync(IEventScheduledBusMessage msg, CancellationToken cancellationToken = default);
    bool AnyListenerFor(string key);

}
