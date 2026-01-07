using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;

namespace LunaticPanel.Core.Messaging.EventBus;

public sealed class EventBus : IEventBus
{

    private readonly IEventBusExchange _eventBusExchange;

    public EventBus(IEventBusExchange eventBusExchange)
    {
        _eventBusExchange = eventBusExchange;
    }



    public Task PublishAsync(IEventBusMessage evt, CancellationToken cancellationToken = default)
        => _eventBusExchange.ExchangeAsync(evt, cancellationToken);


    public IReadOnlyCollection<string> GetAvailableKeys() => _eventBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _eventBusExchange.AnyListenerFor(key);


}
