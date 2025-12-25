namespace LunaticPanel.Core.Messaging.EventBus;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventBusIdAttribute : Attribute
{

    public string EventId { get; }
    public bool IsCrossCircuitReceiver { get; set; } = false;

    public EventBusIdAttribute(string eventId)
    {
        EventId = eventId;
    }
}