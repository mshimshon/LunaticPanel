using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

namespace LunaticPanel.Core.Abstraction.Tools.EventScheduler;

public record EventScheduleObject
{
    public bool RunOnceOnly { get; init; }
    public Func<Task<EventScheduledBusMessageResponse>> Action { get; init; } = default!;
    public string Id { get; }
    public TimeSpan InitialTime { get; }

    public EventScheduleObject(string id, TimeSpan initialTime)
    {
        Id = id;
        InitialTime = initialTime;
    }

}