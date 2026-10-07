using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public interface IEngineBusRegistry
{
    IReadOnlyList<EngineBusHandlerDescriptor> GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    IReadOnlyList<EngineBusHandlerDescriptor> GetAllAvailable();
    bool HasKey(string id);
    void Register(string id, BusHandlerDescriptor handlerEntity);
}
