using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public interface IEventScheduledBusRegistry
{
    EventScheduledBusHandlerDescriptor? GetRegistryFor(string id);
    IReadOnlyList<string> GetAllAvailableIds();
    IReadOnlyList<EventScheduledBusHandlerDescriptor> GetAllAvailable();
    void Register(string id, BusHandlerDescriptor handlerEntity);
    bool HasKey(string id);
}
