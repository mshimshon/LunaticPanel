using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public sealed record EventScheduledBusHandlerDescriptor : BusHandlerDescriptor
{
    public bool RunOnlyOnce { get; init; }
    public bool ScheduleAtStartup { get; init; } = true;
    public bool RunAtStartup { get; init; }
    public TimeSpan Timing { get; init; } = new TimeSpan(0, 5, 0);
    public EventScheduledBusHandlerDescriptor(string id, Type handlerType, EBusLifetime busLifetime) : base(id, handlerType, EBusType.EventBus, busLifetime)
    {
    }
    public static EventScheduledBusHandlerDescriptor Create(BusHandlerDescriptor parent)
        => new(parent.Id, parent.HandlerType, parent.BusLifetime);
}
