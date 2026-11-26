using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public sealed record QueryBusHandlerDescriptor : BusHandlerDescriptor
{
    public QueryBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime, QueryBusKeyAttribute attr) : base(id, handlerType, EBusType.QueryBus, busLifetime, attr)
    {
        Setting = attr;
    }

    public QueryBusKeyAttribute Setting { get; }

    public static QueryBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Key, parent.HandlerType, parent.BusLifetime, (QueryBusKeyAttribute)parent.Attribute);
}
