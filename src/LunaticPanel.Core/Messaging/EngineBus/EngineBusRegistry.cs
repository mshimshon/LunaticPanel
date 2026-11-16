using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;

namespace LunaticPanel.Core.Messaging.EngineBus;

public sealed class EngineBusRegistry : IEngineBusRegistry
{
    private readonly Dictionary<string, List<EngineBusHandlerDescriptor>> _internalRegistryEventTypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Lock _lock = new();

    public IReadOnlyList<EngineBusHandlerDescriptor> GetAllAvailable()
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

    public IReadOnlyList<EngineBusHandlerDescriptor> GetRegistryFor(string id)
    {
        lock (_lock)
        {
            if (_internalRegistryEventTypes.ContainsKey(id))
                return _internalRegistryEventTypes[id].ToList().AsReadOnly();
            return new List<EngineBusHandlerDescriptor>().AsReadOnly();
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
            if (!_internalRegistryEventTypes.TryGetValue(id, out var list))
            {
                list = new List<EngineBusHandlerDescriptor>();
                _internalRegistryEventTypes[id] = new List<EngineBusHandlerDescriptor>();
            }
            if (!list.Any(p => p.HandlerType.FullName == handlerEntity.HandlerType.FullName))
                _internalRegistryEventTypes[id].Add(new(id, handlerEntity.HandlerType, handlerEntity.BusLifetime));
        }
    }
}
