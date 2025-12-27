namespace LunaticPanel.Core.Messaging.Common;

public abstract class BusIdAttribute : Attribute
{
    public string Id { get; }
    protected BusIdAttribute(string id)
    {
        Id = id;
    }
}
