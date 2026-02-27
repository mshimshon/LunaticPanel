namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public sealed record EventScheduledBusMessageData
{
    public DateTime? NextRun { get; init; }
    public bool? ForceReschedule { get; init; }
    public bool? NextRunOnlyOnce { get; init; }
    public TimeSpan? NextTiming { get; init; }
    public Func<CancellationToken, Task> Action { get; }
    public EventScheduledBusMessageData(Func<CancellationToken, Task> action)
    {
        Action = action;
    }

}