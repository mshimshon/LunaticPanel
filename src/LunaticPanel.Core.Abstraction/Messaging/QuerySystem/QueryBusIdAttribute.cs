using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class QueryBusIdAttribute : BusIdAttribute
{
    public QueryBusIdAttribute(string plugin, string action) : base(plugin, action) { }
    public QueryBusIdAttribute(string key) : base(key) { }
    public QueryBusIdAttribute(MessageKey key) : base(key)
    {
    }
}