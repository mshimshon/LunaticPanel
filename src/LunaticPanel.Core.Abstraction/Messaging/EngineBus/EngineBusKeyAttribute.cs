using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EngineBusKeyAttribute : BusKeyAttribute
{
    public EngineBusKeyAttribute(string plugin, string action) : base("enginekey", plugin, action) { }
    public EngineBusKeyAttribute(string key) : base(key) { }
    public EngineBusKeyAttribute(MessageKey key) : base(key)
    {
    }
}
