namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public interface IEventBusExchange
{
    IReadOnlyCollection<string> GetAvailableKeys();
    Task ExchangeAsync(IEventBusMessage evt, CancellationToken cancellationToken = default);
    bool AnyListenerFor(string key);

}
