using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

namespace LunaticPanel.Core.Extensions;

public static class EventScheduleExt
{

    /// <summary>
    /// Checks if the data has any property set that will disable the respect for initial run only once flag and attempt reschedule the event except if DoNotReschedule()
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool HasDisabledRunOnlyOnceForNext(this EventScheduledBusMessageData data)
     => data.NextRunOnlyOnce != default || data.NextRun != default || data.ForceReschedule != default || data.NextTiming != default;

    /// <summary>
    /// This is next running schedule (WARNING: This will not change the frequency only the next execution datetime)
    /// </summary>
    public static EventScheduledBusMessageData NextSchedule(this EventScheduledBusMessageData eventScheduledBusMessageData, TimeSpan timeSpan)
    {
        var delay = timeSpan.TotalMilliseconds < 50
            ? TimeSpan.FromMilliseconds(50)
            : timeSpan;

        var nextSchedule = DateTime.UtcNow.Add(delay);


        return eventScheduledBusMessageData with
        {
            NextRun = nextSchedule
        };
    }


    /// <summary>
    /// This is future frequency timing
    /// </summary>
    public static EventScheduledBusMessageData NextTiming(this EventScheduledBusMessageData eventScheduledBusMessageData, TimeSpan timeSpan)
    {
        var delay = timeSpan.TotalMilliseconds < 50
            ? TimeSpan.FromMilliseconds(50)
            : timeSpan;
        return eventScheduledBusMessageData with
        {
            NextTiming = delay
        };
    }

    /// <summary>
    /// This will reschedule the event for another run regardless if it was run only once or not.
    /// </summary>
    /// <param name="eventScheduledBusMessageData"></param>
    /// <returns></returns>
    public static EventScheduledBusMessageData ForceReschedule(this EventScheduledBusMessageData eventScheduledBusMessageData)
       => eventScheduledBusMessageData with
       {
           ForceReschedule = true
       };

    /// <summary>
    /// This will not reschedule a next run regardless of any other flags until this event is manually registered again.
    /// </summary>
    public static EventScheduledBusMessageData DoNotReschedule(this EventScheduledBusMessageData eventScheduledBusMessageData)
       => eventScheduledBusMessageData with
       {
           ForceReschedule = false
       };

    /// <summary>
    /// Use this method to enable automatically reschedule for future event executions.
    /// </summary>
    /// <remarks>
    /// Be aware that scheduled events may overlap; the event handler is responsible for preventing
    /// concurrent execution when overlap is not allowed, which may require maintaining internal state.
    /// </remarks>
    public static EventScheduledBusMessageData AutoReschedule(this EventScheduledBusMessageData eventScheduledBusMessageData)
       => eventScheduledBusMessageData with
       {
           NextRunOnlyOnce = false // Set if the future schedule automatically reschedule on its own, NOte: internal state tracking is required because two event can overlap it the handler job to ensure its control
       };

    /// <summary>
    /// the next run configured to occur only once.
    /// </summary>
    /// <remarks>
    /// This implies there will be another run of this event and it is the responsibility of the handler to track its state so its knows what to return for next behaviour
    /// </remarks>
    /// <remarks>
    /// Be aware that scheduled events may overlap; the event handler is responsible for preventing
    /// concurrent execution when overlap is not allowed, which may require maintaining internal state.
    /// </remarks>
    public static EventScheduledBusMessageData RunOnlyOnce(this EventScheduledBusMessageData eventScheduledBusMessageData)
       => eventScheduledBusMessageData with
       {
           NextRunOnlyOnce = true
       };



    /// <summary>
    /// Be aware that scheduled events may overlap; the event handler is responsible for preventing
    /// concurrent execution when overlap is not allowed, which may require maintaining internal state.
    /// </summary>
    /// <remarks>
    public static EventScheduledBusMessageData ReplyWithAction(this IEventScheduledBusMessage msg, Func<CancellationToken, Task> action)
    => new EventScheduledBusMessageData(action);
}
