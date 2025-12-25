using LunaticPanel.Core.Messaging.EventBus.Exceptions;

namespace LunaticPanel.Engine.Services.Messaging.EngineBus;

internal class EngineBusRegistry
{
    private readonly Dictionary<string, List<Type>> _internalRegistryEventTypes = new();
    private readonly object _lock = new();

    public IReadOnlyList<string> GetAllAvailableIds()
    {
        lock (_lock)
        {
            return _internalRegistryEventTypes.Keys.ToList().AsReadOnly()!;
        }
    }

    public IReadOnlyList<Type> GetRegistryFor(string id)
    {
        id = id.ToLower();
        lock (_lock)
        {
            return _internalRegistryEventTypes[id]?.ToList()?.AsReadOnly() ?? throw new EventBusNotFoundException(id);
        }
    }

    public void Register(string id, Type handlerEntity)
    {
        id = id.ToLower();

        lock (_lock)
        {
            if (!_internalRegistryEventTypes.TryGetValue(id, out var list))
            {
                list = new List<Type>();
                _internalRegistryEventTypes[id] = new List<Type>();
            }
            if (!list.Contains(handlerEntity))
                _internalRegistryEventTypes[id].Add(handlerEntity);
        }
    }

    public void UnRegister(string id, Type handlerEntity)
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
