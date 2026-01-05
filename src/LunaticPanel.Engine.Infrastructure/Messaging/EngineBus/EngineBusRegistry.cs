using LunaticPanel.Core.Messaging.EventBus.Exceptions;
using LunaticPanel.Engine.Application.Messaging.EngineBus;
using LunaticPanel.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Infrastructure.Messaging.EngineBus;

internal class EngineBusRegistry : IEngineBusRegistry
{
    private readonly Dictionary<string, List<EngineBusHandlerDescriptorEntity>> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<EngineBusHandlerDescriptorEntity> GetAllAvailable()
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

    public IReadOnlyList<EngineBusHandlerDescriptorEntity> GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id]?.ToList()?.AsReadOnly() ?? throw new EventBusNotFoundException(id);
        }
    }

    public bool HasKey(string id)
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.ContainsKey(id);
        }
    }

    public void Register(string id, BusHandlerDescriptorEntity handlerEntity)
    {
        id = id.ToLower();

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var list))
            {
                list = new List<EngineBusHandlerDescriptorEntity>();
                _internalRegistryEventTypes[id] = new List<EngineBusHandlerDescriptorEntity>();
            }
            if (!list.Any(p => p.HandlerType.FullName == handlerEntity.HandlerType.FullName))
                _internalRegistryEventTypes[id].Add(new(id, handlerEntity.HandlerType, handlerEntity.BusType, handlerEntity.Plugin));
        }
    }


}
