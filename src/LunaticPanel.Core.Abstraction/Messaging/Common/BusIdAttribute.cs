namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public abstract class BusIdAttribute : Attribute
{
    public MessageKey Key { get; }
    protected BusIdAttribute(string plugin, string action) : this($"{plugin}.{action}") { }
    protected BusIdAttribute(string key) : this(new MessageKey(key)) { }
    protected BusIdAttribute(MessageKey key)
    {
        Key = key;
    }
}
