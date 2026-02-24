using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using System.Reflection;

namespace LunaticPanel.Core.Messaging.EventScheduledBus;

public sealed class EventScheduledBusRegistry : IEventScheduledBusRegistry
{
    private readonly Dictionary<string, EventScheduledBusHandlerDescriptor> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<EventScheduledBusHandlerDescriptor> GetAllAvailable()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.Select(p => p.Value).ToList().AsReadOnly();
        }

    }

    public IReadOnlyList<string> GetAllAvailableIds()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.Keys.ToList().AsReadOnly()!;
        }
    }

    public EventScheduledBusHandlerDescriptor? GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            if (_internalRegistryEventTypes.ContainsKey(id))
                return _internalRegistryEventTypes[id];
            return default;
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
        var attr = handlerEntity.HandlerType.GetCustomAttribute<EventScheduledBusIdAttribute>(inherit: false);
        if (attr == default)
            throw new InvalidOperationException($"Type {handlerEntity.HandlerType.FullName} MUST implements {nameof(EventBusIdAttribute)}.");

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var exst))
            {
                _internalRegistryEventTypes[id] = new EventScheduledBusHandlerDescriptor(id, handlerEntity.HandlerType, handlerEntity.BusLifetime);
            }
        }
    }
}
