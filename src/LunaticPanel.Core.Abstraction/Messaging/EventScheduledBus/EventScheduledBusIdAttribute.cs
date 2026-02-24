using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventScheduledBusIdAttribute : BusIdAttribute
{
    public bool RunOnlyOnce { get; set; }
    public bool DoNotRegisterAtStartup { get; set; }
    public int Hours { get; set; }
    public int Days { get; set; }
    public int Months { get; set; }
    public int Weeks { get; set; }
    public int Minutes { get; set; } = 5;
    public int Seconds { get; set; }
    public int Miliseconds { get; set; }

    public EventScheduledBusIdAttribute(string plugin, string action) : base(plugin, action) { }
    public EventScheduledBusIdAttribute(string plugin, string action, TimeSpan timing) : base(plugin, action)
    {
        DefineTiming(timing);
    }
    public EventScheduledBusIdAttribute(string key) : base(key) { }
    public EventScheduledBusIdAttribute(string key, TimeSpan timing) : base(key)
    {
        DefineTiming(timing);
    }
    public EventScheduledBusIdAttribute(MessageKey key) : base(key)
    {

    }
    public EventScheduledBusIdAttribute(MessageKey key, TimeSpan timing) : base(key)
    {
        DefineTiming(timing);
    }

    public void DefineTiming(TimeSpan timing)
    {
        Days = timing.Days;
        Hours = timing.Hours;
        Minutes = timing.Minutes;
        Seconds = timing.Seconds;
        Miliseconds = timing.Milliseconds;
        if (timing.TotalMilliseconds < 50)
            Miliseconds = 50;
    }
}