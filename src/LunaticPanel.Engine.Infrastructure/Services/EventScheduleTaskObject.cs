using LunaticPanel.Core.Abstraction.Tools.EventScheduler;

namespace LunaticPanel.Engine.Infrastructure.Services;

public sealed record EventScheduleTaskObject : EventScheduleObject
{
    public Guid ScheduleId { get; init; }
    public EventScheduleTaskObject(string id, TimeSpan initialTime) : base(id, initialTime)
    {
    }
}