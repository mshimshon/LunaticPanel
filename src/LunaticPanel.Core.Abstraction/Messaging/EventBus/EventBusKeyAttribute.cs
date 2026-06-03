using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventBusKeyAttribute : BusKeyAttribute
{

    public EventBusSpreadType CrossCircuitReceiver { get; set; } = EventBusSpreadType.SelfContained;

    public EventBusKeyAttribute(string plugin, string action) : base("eventkey", plugin, action) { }
    public EventBusKeyAttribute(string key) : base(key) { }
    public EventBusKeyAttribute(MessageKey key) : base(key)
    {

    }
}