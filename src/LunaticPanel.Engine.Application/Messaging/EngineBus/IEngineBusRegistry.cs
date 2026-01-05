using LunaticPanel.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Application.Messaging.EngineBus;

public interface IEngineBusRegistry
{
    IReadOnlyList<EngineBusHandlerDescriptorEntity> GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    IReadOnlyList<EngineBusHandlerDescriptorEntity> GetAllAvailable();
    bool HasKey(string id);
    void Register(string id, BusHandlerDescriptorEntity handlerEntity);

}
