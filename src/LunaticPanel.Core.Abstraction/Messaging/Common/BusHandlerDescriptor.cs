namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public record BusHandlerDescriptor(
        string Id,
        Type HandlerType,
        EBusType BusType
    )
{
}
