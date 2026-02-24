namespace LunaticPanel.Core.Abstraction.Tools.EventScheduler;

public interface IEventScheduler
{
    Guid Register(EventScheduleObject task);
    void UnRegister(Guid id);
}
