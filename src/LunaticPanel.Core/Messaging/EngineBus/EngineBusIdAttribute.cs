namespace LunaticPanel.Core.Messaging.EngineBus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EngineBusIdAttribute : Attribute
{
    public string Id { get; }
    public EngineBusIdAttribute(string id)
    {
        Id = id;
    }
}
