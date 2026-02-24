using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public sealed record EngineBusHandlerDescriptor : BusHandlerDescriptor
{
    public EngineBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime) : base(id, handlerType, EBusType.EngineBus, busLifetime)
    {
    }
    public static EngineBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType, parent.BusLifetime);
}
