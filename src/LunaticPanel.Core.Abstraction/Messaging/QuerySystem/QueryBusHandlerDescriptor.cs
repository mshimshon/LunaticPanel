using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public sealed record QueryBusHandlerDescriptor : BusHandlerDescriptor
{
    public QueryBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime) : base(id, handlerType, EBusType.QueryBus, busLifetime)
    {
    }
    public static QueryBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType, parent.BusLifetime);
}