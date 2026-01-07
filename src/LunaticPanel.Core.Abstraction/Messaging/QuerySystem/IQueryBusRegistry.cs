using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public interface IQueryBusRegistry
{
    QueryBusHandlerDescriptor? GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    IReadOnlyList<QueryBusHandlerDescriptor> GetAllAvailable();
    void Register(string id, BusHandlerDescriptor handlerEntity);
    bool HasKey(string id);
}
