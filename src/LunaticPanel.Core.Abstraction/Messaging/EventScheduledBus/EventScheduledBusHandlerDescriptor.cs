using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public sealed record EventScheduledBusHandlerDescriptor : BusHandlerDescriptor
{
    public EventScheduledBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime) : base(id, handlerType, EBusType.EventBus, busLifetime)
    {
    }
    public static EventScheduledBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType, parent.BusLifetime);
}
