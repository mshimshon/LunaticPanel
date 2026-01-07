using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Messaging.EventBus;

public sealed class EventBus : IEventBus, IEventBusReceiver
{

    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBusRegistry _eventBusRegistry;
    private readonly IEventBusExchange _eventBusExchange;
    private readonly ICircuitRegistry _circuitRegistry;

    public EventBus(IServiceProvider serviceProvider, IEventBusRegistry eventBusRegistry, IEventBusExchange eventBusExchange, ICircuitRegistry circuitRegistry)
    {
        _serviceProvider = serviceProvider;
        _eventBusRegistry = eventBusRegistry;
        _eventBusExchange = eventBusExchange;
        _circuitRegistry = circuitRegistry;
    }

    private static Task ExecuteHandler(IEventBusMessage evt, IServiceProvider serviceProvider, Type handlerType)
    {
        var handler = serviceProvider.GetRequiredService(handlerType) as IEventBusHandler;
        return handler!.HandleAsync(evt);
    }

    public Task PublishAsync(IEventBusMessage evt, CancellationToken cancellationToken = default)
        => _eventBusExchange.ExchangeAsync(evt, cancellationToken);


    public IReadOnlyCollection<string> GetAvailableKeys() => _eventBusExchange.GetAvailableKeys();
    public IReadOnlyCollection<Type> GetAllHandlersByEventId(MessageKey messageKey) =>
        _eventBusRegistry.GetRegistryFor(messageKey.ToString()).Select(p => p.HandlerType).ToList();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _eventBusExchange.AnyListenerFor(key);

    public Task IncomingMessageAsync(IEventBusMessage eventBusMessage, CancellationToken cancellationToken = default)
    {
        string id = eventBusMessage.GetId();
        Guid? circuitIdOrigin = eventBusMessage.GetOriginCircuitId();
        if (circuitIdOrigin == default) return Task.CompletedTask;
        var registry = _eventBusRegistry;

        var handlers = registry.GetRegistryFor(id);
        if (handlers.Count <= 0) return Task.CompletedTask;

        List<Task> handlerTasks = new();
        foreach (var handler in handlers)
            if (!handler.IsCrossCircuitType)
                if (circuitIdOrigin == _circuitRegistry.CurrentCircuit.CircuitId)
                    handlerTasks.Add(ExecuteHandler(eventBusMessage, _serviceProvider, handler.HandlerType));
                else continue;
            else
                handlerTasks.Add(ExecuteHandler(eventBusMessage, _serviceProvider, handler.HandlerType));

        return Task.WhenAll(handlerTasks);
    }
    public IReadOnlyCollection<string> WhatDoYouListenFor() => _eventBusRegistry.GetAllAvailableIds();
    public bool DoYouListenTo(string key) => _eventBusRegistry.HasKey(key);
}
