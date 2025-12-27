using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Messaging.Enums;

namespace LunaticPanel.Engine.Domain.Messaging.Entities;

public sealed record EngineBusHandlerDescriptorEntity : BusHandlerDescriptorEntity
{
    public EngineBusHandlerDescriptorEntity(string Id, Type HandlerType, EBusType BusType, IPlugin? Plugin) : base(Id, HandlerType, BusType, Plugin)
    {
    }
}
