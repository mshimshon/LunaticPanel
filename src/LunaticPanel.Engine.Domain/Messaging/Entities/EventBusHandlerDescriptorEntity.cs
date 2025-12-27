using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Messaging.Enums;

namespace LunaticPanel.Engine.Domain.Messaging.Entities;

public sealed record EventBusHandlerDescriptorEntity : BusHandlerDescriptorEntity
{
    public EventBusHandlerDescriptorEntity(string Id, Type HandlerType, EBusType BusType, IPlugin? Plugin) : base(Id, HandlerType, BusType, Plugin)
    {
    }
    public bool IsCrossCircuitType { get; init; }
}
