using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public sealed record EventScheduledBusHandlerDescriptor : BusHandlerDescriptor
{
    public TimeSpan Timing { get; init; }
    public EventScheduledBusKeyAttribute Setting { get; }

    public EventScheduledBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime, EventScheduledBusKeyAttribute setting) : base(id, handlerType, EBusType.EventBus, busLifetime, setting)
    {
        Setting = setting;
        Timing = setting.GetTiming();
    }
    public static EventScheduledBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Key, parent.HandlerType, parent.BusLifetime, (EventScheduledBusKeyAttribute)parent.Attribute);
}
