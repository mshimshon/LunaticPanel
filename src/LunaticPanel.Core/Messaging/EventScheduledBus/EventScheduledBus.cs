using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

namespace LunaticPanel.Core.Messaging.EventScheduledBus;

public sealed class EventScheduledBus : IEventScheduledBus
{

    private readonly IEventScheduledBusExchange _eventScheduledBusExchange;

    public EventScheduledBus(IEventScheduledBusExchange eventBusExchange)
    {
        _eventScheduledBusExchange = eventBusExchange;
    }


    public Task<EventScheduledBusMessageResponse> PublishAsync(IEventScheduledBusMessage msg, CancellationToken cancellationToken = default)
      => _eventScheduledBusExchange.ExchangeAsync(msg, cancellationToken);

    public IReadOnlyCollection<string> GetAvailableKeys() => _eventScheduledBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _eventScheduledBusExchange.AnyListenerFor(key);


}
