using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.EventBus.Exceptions;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Domain.Messaging.Entities;
using System.Reflection;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Event;

internal class EventBusRegistry : IEventBusRegistry
{
    private readonly Dictionary<string, List<EventBusHandlerDescriptorEntity>> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<string> GetAllAvailableIds()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.Keys.ToList().AsReadOnly()!;
        }
    }

    public IReadOnlyList<EventBusHandlerDescriptorEntity> GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id]?.ToList()?.AsReadOnly() ?? throw new EventBusNotFoundException(id);
        }
    }

    public void Register(string id, BusHandlerDescriptorEntity handlerEntity)
    {
        id = id.ToLower();
        var attr = handlerEntity.HandlerType.GetCustomAttribute<EventBusIdAttribute>(inherit: false);
        if (attr == default)
            throw new InvalidOperationException($"Type {handlerEntity.HandlerType.FullName} MUST implements {nameof(EventBusIdAttribute)}.");

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var list))
            {
                list = new List<EventBusHandlerDescriptorEntity>();
                _internalRegistryEventTypes[id] = new List<EventBusHandlerDescriptorEntity>();
            }
            if (!list.Any(p => p.HandlerType.FullName == handlerEntity.HandlerType.FullName))
                _internalRegistryEventTypes[id].Add(new(id, handlerEntity.HandlerType, handlerEntity.BusType, handlerEntity.Plugin)
                {
                    IsCrossCircuitType = attr.IsCrossCircuitReceiver
                });
        }
    }
}
