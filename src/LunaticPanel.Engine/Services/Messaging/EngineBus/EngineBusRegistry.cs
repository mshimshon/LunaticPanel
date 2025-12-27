using LunaticPanel.Core.Messaging.EventBus.Exceptions;
using LunaticPanel.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Services.Messaging.EngineBus;

internal class EngineBusRegistry
{
    private readonly Dictionary<string, List<EngineBusHandlerDescriptorEntity>> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

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
