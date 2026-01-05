using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.Common;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Engine.Application.Circuit;
using LunaticPanel.Engine.Application.Messaging.Event;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Event;

internal class EventBus : IEventBus
{

    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBusRegistry _eventBusRegistry;
    private readonly ICircuitControl _circuitControl;

    public EventBus(IServiceProvider serviceProvider, IEventBusRegistry eventBusRegistry, ICircuitControl circuitControl)
    {
        _serviceProvider = serviceProvider;
        _eventBusRegistry = eventBusRegistry;
        _circuitControl = circuitControl;
    }

    private static Task ExecuteHandler(IEventBusMessage evt, IServiceProvider serviceProvider, Type handlerType)
    {
        var handler = serviceProvider.GetRequiredService(handlerType) as IEventBusHandler;
        return handler!.HandleAsync(evt);
    }

    public Task PublishAsync(IEventBusMessage evt)
    {
        string id = evt.GetId();
        var registry = _eventBusRegistry;
        var handlers = registry.GetRegistryFor(id);
        List<Task> handlerTasks = new();
        bool hasCrossCircuitEvents = handlers.Any(p => p.IsCrossCircuitType);
        foreach (var item in handlers.Where(p => !p.IsCrossCircuitType))
            if (item.Plugin != default)
            {
                var pluginType = item.Plugin.GetType();
                var serviceType = typeof(IPluginService<>).MakeGenericType(pluginType);
                var pluginService = _serviceProvider.GetRequiredService(serviceType) as IPluginService;
                handlerTasks.Add(ExecuteHandler(evt, pluginService!, item.HandlerType));
            }
            else
                handlerTasks.Add(ExecuteHandler(evt, _serviceProvider, item.HandlerType));

        if (hasCrossCircuitEvents)
            foreach (var circuit in _circuitControl.GetActiveCircuits())
                foreach (var handlerType in handlers.Where(p => p.IsCrossCircuitType))
                    if (handlerType.Plugin != default)
                    {
                        var pluginType = handlerType.Plugin.GetType();
                        var serviceType = typeof(IPluginService<>).MakeGenericType(pluginType);
                        var pluginService = circuit.ServiceProvider().GetRequiredService(serviceType) as IPluginService;
                        var pluginSp = pluginService!.GetRequired<IServiceProvider>()!;
                        handlerTasks.Add(ExecuteHandler(evt, pluginSp, handlerType.HandlerType));
                    }
                    else
                        handlerTasks.Add(ExecuteHandler(evt, circuit.ServiceProvider(), handlerType.HandlerType));

        return Task.WhenAll(handlerTasks);
    }


    public IReadOnlyCollection<string> GetAvailableKeys() => _eventBusRegistry.GetAllAvailableIds();
    public IReadOnlyCollection<Type> GetAllHandlersByEventId(MessageKey messageKey) =>
        _eventBusRegistry.GetRegistryFor(messageKey.ToString()).Select(p => p.HandlerType).ToList();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _eventBusRegistry.HasKey(key);
}
