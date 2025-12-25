using GameServerManager.Engine.Domain.Messaging.Entities;
using LunaticPanel.Core.Messaging.EventBus.Exceptions;
using LunaticPanel.Engine.Application.Messaging.Event;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Event;

public class EventBusRegistry : IEventBusRegistry
{
    private readonly Dictionary<string, List<EventTypeEntity>> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<string> GetAllAvailableIds()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.Keys.ToList().AsReadOnly()!;
        }
    }

    public IReadOnlyList<EventTypeEntity> GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id]?.ToList()?.AsReadOnly() ?? throw new EventBusNotFoundException(id);
        }
    }

    public void Register(string id, EventTypeEntity handlerEntity)
    {
        id = id.ToLower();

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var list))
            {
                list = new List<EventTypeEntity>();
                _internalRegistryEventTypes[id] = new List<EventTypeEntity>();
            }
            if (!list.Contains(handlerEntity))
                _internalRegistryEventTypes[id].Add(handlerEntity);
        }
    }

    public void UnRegister(string id, EventTypeEntity handlerEntity)
    {
        id = id.ToLower();

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var list))
                return;
            _internalRegistryEventTypes[id].Remove(handlerEntity);
        }
    }
}
