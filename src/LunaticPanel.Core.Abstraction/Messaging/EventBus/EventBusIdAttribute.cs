using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventBusIdAttribute : BusIdAttribute
{

    public bool IsCrossCircuitReceiver { get; set; } = false;
    public EventBusIdAttribute(string plugin, string action) : base(plugin, action) { }
    public EventBusIdAttribute(string key) : base(key) { }
    public EventBusIdAttribute(MessageKey key) : base(key)
    {

    }
}