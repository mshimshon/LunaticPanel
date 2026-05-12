namespace LunaticPanel.Core.Abstraction.Tools.EventScheduler;

public record EventScheduleObject
{
    public bool RunOnceOnly { get; init; }
    public string Id { get; }
    public TimeSpan InitialTime { get; }

    public EventScheduleObject(string id, TimeSpan initialTime)
    {
        Id = id;
        InitialTime = initialTime;
    }

}