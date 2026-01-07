using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using System.Reflection;

namespace LunaticPanel.Core.Messaging.EventBus;

public sealed class EventBusRegistry : IEventBusRegistry
{
    private readonly Dictionary<string, List<EventBusHandlerDescriptor>> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<EventBusHandlerDescriptor> GetAllAvailable()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.SelectMany(p => p.Value).ToList().AsReadOnly();
        }

    }

    public IReadOnlyList<string> GetAllAvailableIds()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.Keys.ToList().AsReadOnly()!;
        }
    }

    public IReadOnlyList<EventBusHandlerDescriptor> GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id]?.ToList()?.AsReadOnly() ?? new List<EventBusHandlerDescriptor>().AsReadOnly();
        }
    }

    public bool HasKey(string id)
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.ContainsKey(id);
        }
    }

    public void Register(string id, BusHandlerDescriptor handlerEntity)
    {
        id = id.ToLower();
        var attr = handlerEntity.HandlerType.GetCustomAttribute<EventBusIdAttribute>(inherit: false);
        if (attr == default)
            throw new InvalidOperationException($"Type {handlerEntity.HandlerType.FullName} MUST implements {nameof(EventBusIdAttribute)}.");

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var list))
            {
                list = new List<EventBusHandlerDescriptor>();
                _internalRegistryEventTypes[id] = new List<EventBusHandlerDescriptor>();
            }
            if (!list.Any(p => p.HandlerType.FullName == handlerEntity.HandlerType.FullName))
                _internalRegistryEventTypes[id].Add(new(id, handlerEntity.HandlerType)
                {
                    IsCrossCircuitType = attr.IsCrossCircuitReceiver
                });
        }
    }
}
