using LunaticPanel.Core.Messaging.QuerySystem.Exceptions;
using LunaticPanel.Engine.Application.Messaging.Query;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Query;

internal class QueryBusRegistry : IQueryBusRegistry
{
    private readonly Dictionary<string, Type> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<string> GetAllAvailableIds()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.Keys.ToList().AsReadOnly()!;
        }
    }

    public Type GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id] ?? throw new QueryBusNotFoundException(id);
        }
    }

    public void Register(string id, Type handlerType)
    {
        id = id.ToLower();

        lock (_lock)
        {
            if (_internalRegistryEventTypes.ContainsKey(id))
                throw new QueryBusMultipleHandlerException(id);
            _internalRegistryEventTypes[id] = handlerType;
        }
    }

    public void UnRegister(string id)
    {
        id = id.ToLower();

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.ContainsKey(id))
                _internalRegistryEventTypes.Remove(id);
        }
    }
}
