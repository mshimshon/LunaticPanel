using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Messaging.EventBus;

public class EventBusReceiver : IEventBusReceiver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBusRegistry _eventBusRegistry;
    private readonly ICircuitRegistry _circuitRegistry;

    public EventBusReceiver(IServiceProvider serviceProvider, IEventBusRegistry eventBusRegistry, ICircuitRegistry circuitRegistry)
    {
        _serviceProvider = serviceProvider;
        _eventBusRegistry = eventBusRegistry;
        _circuitRegistry = circuitRegistry;
    }
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

    private static Task ExecuteHandler(IEventBusMessage evt, IServiceProvider serviceProvider, Type handlerType)
    {
        var handler = serviceProvider.GetRequiredService(handlerType) as IEventBusHandler;
        return handler!.HandleAsync(evt);
    }
}
