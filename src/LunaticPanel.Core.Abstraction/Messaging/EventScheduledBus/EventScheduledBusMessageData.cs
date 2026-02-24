namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public sealed class EventScheduledBusMessageData
{
    public DateTime? NextRun { get; init; }
    public Func<CancellationToken, Task> Action { get; }
    public EventScheduledBusMessageData(Func<CancellationToken, Task> action)
    {
        Action = action;
    }
}
