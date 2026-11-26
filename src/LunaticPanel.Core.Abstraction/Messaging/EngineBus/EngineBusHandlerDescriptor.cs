using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public sealed record EngineBusHandlerDescriptor : BusHandlerDescriptor
{
    public EngineBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime, EngineBusKeyAttribute setting) : base(id, handlerType, EBusType.EngineBus, busLifetime, setting)
    {
        Setting = setting;
    }

    public EngineBusKeyAttribute Setting { get; }

    public static EngineBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Key, parent.HandlerType, parent.BusLifetime, (EngineBusKeyAttribute)parent.Attribute);
}
