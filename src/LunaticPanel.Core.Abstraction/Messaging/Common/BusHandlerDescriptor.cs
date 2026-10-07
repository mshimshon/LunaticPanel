namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public record BusHandlerDescriptor(
        string Key,
        Type HandlerType,
        EBusType BusType, EBusLifetime BusLifetime
    )
{
}
