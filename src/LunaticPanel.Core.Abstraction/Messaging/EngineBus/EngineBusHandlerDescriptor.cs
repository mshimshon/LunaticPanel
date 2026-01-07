using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public sealed record EngineBusHandlerDescriptor : BusHandlerDescriptor
{
    public EngineBusHandlerDescriptor(string id, Type handlerType) : base(id, handlerType, EBusType.EngineBus)
    {
    }
    public static EngineBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType);
}
