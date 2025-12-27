using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EventBus;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventBusIdAttribute : BusIdAttribute
{

    public bool IsCrossCircuitReceiver { get; set; } = false;

    public EventBusIdAttribute(string id) : base(id)
    {

    }
}