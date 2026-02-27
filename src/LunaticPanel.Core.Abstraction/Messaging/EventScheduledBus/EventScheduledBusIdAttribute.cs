using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;


/// <summary>
/// Specifies scheduling metadata for a bus event, allowing configuration of execution timing, frequency, and startup
/// behavior.
/// </summary>
/// <remarks>Apply this attribute to a class to define how and when the associated bus event should be executed.
/// You can configure the event to run only once, schedule it to run at application startup, or specify a recurring
/// interval in weeks or as a TimeSpan. If no timing is specified, the default execution interval is 5 minutes. This
/// attribute is not intended for use on multiple classes simultaneously (AllowMultiple = false).</remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventScheduledBusIdAttribute : BusIdAttribute
{
    public bool RunOnlyOnce { get; set; }
    public bool ScheduleAtStartup { get; set; } = true;
    public bool RunAtStartup { get; set; }
    public int Weeks { get; set; }

    private TimeSpan _timing = new TimeSpan();

    public EventScheduledBusIdAttribute(string plugin, string action) : base(plugin, action) { }
    public EventScheduledBusIdAttribute(string plugin, string action, int days, int hours, int minutes, int seconds, int milliseconds) : this(plugin, action)
    {
        _timing = new TimeSpan(days, hours, minutes, seconds, milliseconds);
    }
    public EventScheduledBusIdAttribute(string plugin, string action, int minutes, int seconds, int milliseconds) : this(plugin, action, 0, 0, minutes, seconds, milliseconds) { }
    public EventScheduledBusIdAttribute(string plugin, string action, int hours, int minutes, int seconds, int milliseconds) : this(plugin, action, 0, hours, minutes, seconds, milliseconds) { }
    public EventScheduledBusIdAttribute(string plugin, string action, int minutes, int seconds) : this(plugin, action, 0, 0, minutes, seconds, 0) { }


    public EventScheduledBusIdAttribute(string key) : base(key) { }
    public EventScheduledBusIdAttribute(string key, int days, int hours, int minutes, int seconds, int milliseconds) : this(key)
    {
        _timing = new TimeSpan(days, hours, minutes, seconds, milliseconds);
    }
    public EventScheduledBusIdAttribute(string key, int minutes, int seconds, int milliseconds) : this(key, 0, 0, minutes, seconds, milliseconds) { }
    public EventScheduledBusIdAttribute(string key, int hours, int minutes, int seconds, int milliseconds) : this(key, 0, hours, minutes, seconds, milliseconds) { }
    public EventScheduledBusIdAttribute(string key, int minutes, int seconds) : this(key, 0, 0, minutes, seconds, 0) { }


    public EventScheduledBusIdAttribute(MessageKey key) : base(key)
    {

    }
    public EventScheduledBusIdAttribute(MessageKey key, int days, int hours, int minutes, int seconds, int milliseconds) : this(key)
    {
        _timing = new TimeSpan(days, hours, minutes, seconds, milliseconds);
    }
    public EventScheduledBusIdAttribute(MessageKey key, int minutes, int seconds, int milliseconds) : this(key, 0, 0, minutes, seconds, milliseconds) { }
    public EventScheduledBusIdAttribute(MessageKey key, int hours, int minutes, int seconds, int milliseconds) : this(key, 0, hours, minutes, seconds, milliseconds) { }
    public EventScheduledBusIdAttribute(MessageKey key, int minutes, int seconds) : this(key, 0, 0, minutes, seconds, 0) { }


    public TimeSpan GetTiming()
    {
        var finalTiming = new TimeSpan((Weeks * 7), 0, 0, 0);
        finalTiming.Add(_timing);
        if (finalTiming.TotalMilliseconds <= 0)
            return new TimeSpan(0, 5, 0);
        else if (finalTiming.TotalMilliseconds <= 50)
            return new TimeSpan(0, 5, 0, 0, 50);
        return finalTiming;
    }
}