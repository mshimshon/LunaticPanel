using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public sealed record EventBusHandlerDescriptor : BusHandlerDescriptor
{
    public EventBusSpreadType CrossCircuitType { get; set; }
    public EventBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime) : base(id, handlerType, EBusType.EventBus, busLifetime)
    {
    }
    public static EventBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType, parent.BusLifetime);
}
