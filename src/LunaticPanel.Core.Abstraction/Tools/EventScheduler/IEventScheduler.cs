namespace LunaticPanel.Core.Abstraction.Tools.EventScheduler;

public interface IEventScheduler
{
    Guid Register(EventScheduleObject task, bool runNow);
    void UnRegister(Guid id);
}
