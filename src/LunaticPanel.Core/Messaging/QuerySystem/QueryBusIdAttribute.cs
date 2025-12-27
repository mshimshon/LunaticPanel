using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.QuerySystem;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class QueryBusIdAttribute : BusIdAttribute
{
    public QueryBusIdAttribute(string id) : base(id)
    {
    }
}