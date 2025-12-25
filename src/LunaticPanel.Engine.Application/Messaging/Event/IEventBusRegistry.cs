using GameServerManager.Engine.Domain.Messaging.Entities;

namespace LunaticPanel.Engine.Application.Messaging.Event;

public interface IEventBusRegistry
{
    IReadOnlyList<EventTypeEntity> GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    void Register(string id, EventTypeEntity handlerEntity);
    void UnRegister(string id, EventTypeEntity handlerEntity);
}
