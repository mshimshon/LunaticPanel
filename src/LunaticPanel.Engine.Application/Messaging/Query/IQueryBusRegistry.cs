using LunaticPanel.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Application.Messaging.Query;

public interface IQueryBusRegistry
{
    QueryBusHandlerDescriptorEntity GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    IReadOnlyList<QueryBusHandlerDescriptorEntity> GetAllAvailable();
    void Register(string id, BusHandlerDescriptorEntity handlerEntity);
    bool HasKey(string id);
}
