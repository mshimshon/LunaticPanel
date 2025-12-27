using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Messaging.Enums;

namespace LunaticPanel.Engine.Domain.Messaging.Entities;

public record BusHandlerDescriptorEntity(
        string Id,
        Type HandlerType,
        EBusType BusType,
        IPlugin? Plugin
    )
{
}
