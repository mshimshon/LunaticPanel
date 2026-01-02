using LunaticPanel.Core.Messaging.QuerySystem.Exceptions;
using LunaticPanel.Engine.Application.Messaging.Query;
using LunaticPanel.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Query;

internal class QueryBusRegistry : IQueryBusRegistry
{
    private readonly Dictionary<string, QueryBusHandlerDescriptorEntity> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<QueryBusHandlerDescriptorEntity> GetAllAvailable()
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

    public QueryBusHandlerDescriptorEntity GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id] ?? throw new QueryBusNotFoundException(id);
        }
    }

    public void Register(string id, BusHandlerDescriptorEntity handlerEntity)
    {
        id = id.ToLower();

        lock (_lock)
        {
            if (_internalRegistryEventTypes.ContainsKey(id))
                throw new QueryBusMultipleHandlerException(id);
            _internalRegistryEventTypes[id] = new(id, handlerEntity.HandlerType, handlerEntity.BusType, handlerEntity.Plugin);
        }
    }


}
