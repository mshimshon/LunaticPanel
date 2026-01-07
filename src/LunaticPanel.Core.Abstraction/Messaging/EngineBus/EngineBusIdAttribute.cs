using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EngineBusIdAttribute : BusIdAttribute
{
    public EngineBusIdAttribute(string plugin, string action) : base(plugin, action) { }
    public EngineBusIdAttribute(string key) : base(key) { }
    public EngineBusIdAttribute(MessageKey key) : base(key)
    {
    }
}
