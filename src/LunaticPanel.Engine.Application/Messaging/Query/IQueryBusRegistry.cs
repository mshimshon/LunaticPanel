using LunaticPanel.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Application.Messaging.Query;

public interface IQueryBusRegistry
{
    QueryBusHandlerDescriptorEntity GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    void Register(string id, BusHandlerDescriptorEntity handlerEntity);
}
