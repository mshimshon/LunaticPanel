using LunaticPanel.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Application.Messaging.Event;

public interface IEventBusRegistry
{
    IReadOnlyList<EventBusHandlerDescriptorEntity> GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    IReadOnlyList<EventBusHandlerDescriptorEntity> GetAllAvailable();
    void Register(string id, BusHandlerDescriptorEntity handlerEntity);
}
