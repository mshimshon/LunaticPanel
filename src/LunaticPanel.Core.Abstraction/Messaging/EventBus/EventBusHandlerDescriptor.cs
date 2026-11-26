using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public sealed record EventBusHandlerDescriptor : BusHandlerDescriptor
{
    public EventBusKeyAttribute Setting { get; }

    public EventBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime, EventBusKeyAttribute setting) : base(id, handlerType, EBusType.EventBus, busLifetime, setting)
    {
        Setting = setting;
    }
    public static EventBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Key, parent.HandlerType, parent.BusLifetime, (EventBusKeyAttribute)parent.Attribute);
}
