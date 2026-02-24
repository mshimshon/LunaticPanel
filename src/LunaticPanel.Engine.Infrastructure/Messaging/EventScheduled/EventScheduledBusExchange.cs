using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Engine.Infrastructure.Messaging.Query;

namespace LunaticPanel.Engine.Infrastructure.Messaging.EventScheduled;

internal class EventScheduledBusExchange : IEventScheduledBusExchange
{
    private readonly ICircuitRegistry _circuitRegistry;
    private readonly IEventScheduledBusReceiver _eventScheduledBusReceiver;

    private Guid CircuitId => _circuitRegistry.CurrentCircuit.CircuitId;
    public EventScheduledBusExchange(ICircuitRegistry circuitRegistry, IEventScheduledBusReceiver eventScheduledBusReceiver)
    {
        _circuitRegistry = circuitRegistry;
        _eventScheduledBusReceiver = eventScheduledBusReceiver;
    }

    public bool AnyListenerFor(string key)
    {
        key = key.ToLower();
        var result = _eventScheduledBusReceiver.DoYouListenTo(key);
        if (result) return result;
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;

            var pluginBusReceiver = item.Value.GetRequired<IEventScheduledBusReceiver>();
            result = pluginBusReceiver.DoYouListenTo(key);
            if (result) return result;
        }
        return false;
    }

    public async Task<EventScheduledBusMessageResponse> ExchangeAsync(IEventScheduledBusMessage msg, CancellationToken cancellationToken = default)
    {
        var message = msg;
        message.SetOriginCircuitId(CircuitId);

        var result = await _eventScheduledBusReceiver.IncomingMessageAsync(message, cancellationToken);
        if (result != default) return result;

        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            var _pluginBusReceiver = item.Value.GetRequired<IEventScheduledBusReceiver>();
            result = await _pluginBusReceiver.IncomingMessageAsync(message, cancellationToken);
            if (result != default) return result;
        }
        return new(new((ct) => Task.CompletedTask))
        {
            Error = new("INTERNAL", $"No Scheduled Handler for {msg.GetId()}"),
            Origin = nameof(QueryBusExchange)
        };
    }

    public IReadOnlyCollection<string> GetAvailableKeys()
    {
        var hostListeningList = _eventScheduledBusReceiver.WhatDoYouListenFor();
        List<string> result = new();
        result.AddRange(hostListeningList);
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;
            var pluginBusReceiver = item.Value.GetRequired<IEventScheduledBusReceiver>();
            var handlerResults = pluginBusReceiver.WhatDoYouListenFor();
            result.AddRange(handlerResults.Where(p => !result.Contains(p)));

        }
        return result.AsReadOnly();
    }
}
