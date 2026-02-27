using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Tools.EventScheduler;

namespace LunaticPanel.Engine.Infrastructure.Services;

public sealed record EventScheduleTaskObject : EventScheduleObject
{
    public Func<Task<EventScheduledBusMessageResponse>> Action { get; init; } = default!;
    public Guid ScheduleId { get; init; }
    public EventScheduleTaskObject(string id, TimeSpan initialTime) : base(id, initialTime)
    {
    }
}