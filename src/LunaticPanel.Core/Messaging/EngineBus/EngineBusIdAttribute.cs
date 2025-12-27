using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EngineBus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EngineBusIdAttribute : BusIdAttribute
{
    public EngineBusIdAttribute(string id) : base(id)
    {
    }
}
