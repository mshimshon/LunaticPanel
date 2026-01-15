using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public sealed record EventBusHandlerDescriptor : BusHandlerDescriptor
{
    public EventBusSpreadType CrossCircuitType { get; set; }
    public EventBusHandlerDescriptor(string id, Type handlerType) : base(id, handlerType, EBusType.EventBus)
    {
    }
    public static EventBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType);
}
