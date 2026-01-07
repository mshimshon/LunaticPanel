using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Event;

internal class EventBusExchange : IEventBusExchange
{
    private readonly ICircuitRegistry _circuitRegistry;
    private readonly IEventBusReceiver _eventBusReceiver;

    private Guid CircuitId => _circuitRegistry.CurrentCircuit.CircuitId;
    public EventBusExchange(ICircuitRegistry circuitRegistry, IEventBusReceiver eventBusReceiver)
    {
        _circuitRegistry = circuitRegistry;
        _eventBusReceiver = eventBusReceiver;
    }
    public bool AnyListenerFor(string key)
    {
        key = key.ToLower();
        var result = _eventBusReceiver.DoYouListenTo(key);
        if (result) return result;
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;

            var pluginBusReceiver = item.Value.GetRequired<IEventBusReceiver>();
            result = pluginBusReceiver.DoYouListenTo(key);
            if (result) return result;
        }
        return false;
    }
    public Task ExchangeAsync(IEventBusMessage evt, CancellationToken cancellationToken = default)
    {
        evt.SetOriginCircuitId(CircuitId);
        var message = evt;
        _ = _eventBusReceiver.IncomingMessageAsync(message, cancellationToken);

        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            var _pluginBusReceiver = item.Value.GetRequired<IEventBusReceiver>();
            _ = _pluginBusReceiver.IncomingMessageAsync(message, cancellationToken);
        }
        return Task.CompletedTask;
    }

    public IReadOnlyCollection<string> GetAvailableKeys()
    {
        var hostListeningList = _eventBusReceiver.WhatDoYouListenFor();
        List<string> result = new();
        result.AddRange(hostListeningList);
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;
            var pluginBusReceiver = item.Value.GetRequired<IEventBusReceiver>();
            var handlerResults = pluginBusReceiver.WhatDoYouListenFor();
            result.AddRange(handlerResults.Where(p => !result.Contains(p)));

        }
        return result.AsReadOnly();
    }
}
