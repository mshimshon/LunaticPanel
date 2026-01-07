using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem.Exceptions;

namespace LunaticPanel.Core.Messaging.QuerySystem;

public sealed class QueryBusRegistry : IQueryBusRegistry
{
    private readonly Dictionary<string, QueryBusHandlerDescriptor> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<QueryBusHandlerDescriptor> GetAllAvailable()
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

    public QueryBusHandlerDescriptor? GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id] ?? default;
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

        lock (_lock)
        {
            if (_internalRegistryEventTypes.ContainsKey(id))
                throw new QueryBusMultipleHandlerException(id);
            _internalRegistryEventTypes[id] = new(id, handlerEntity.HandlerType);
        }
    }


}
