using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class QueryBusKeyAttribute : BusKeyAttribute
{
    public QueryBusKeyAttribute(string plugin, string action) : base("querykey", plugin, action) { }
    public QueryBusKeyAttribute(string key) : base(key) { }
    public QueryBusKeyAttribute(MessageKey key) : base(key)
    {
    }
}