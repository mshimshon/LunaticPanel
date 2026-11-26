using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using System.Reflection;

namespace LunaticPanel.Core.Messaging.EventScheduledBus;

public sealed class EventScheduledBusRegistry : IEventScheduledBusRegistry
{
    private readonly Dictionary<string, EventScheduledBusHandlerDescriptor> _internalRegistryEventTypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Lock _lock = new();

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
        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var exst))
            {
                _internalRegistryEventTypes[id] = EventScheduledBusHandlerDescriptor.Create(handlerEntity);
            }
        }
    }
}
