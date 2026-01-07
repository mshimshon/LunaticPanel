using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public sealed record QueryBusHandlerDescriptor : BusHandlerDescriptor
{
    public QueryBusHandlerDescriptor(string id, Type handlerType) : base(id, handlerType, EBusType.QueryBus)
    {
    }
    public static QueryBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType);
}