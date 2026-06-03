namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public abstract class BusKeyAttribute : Attribute
{
    public MessageKey Key { get; }
    public EBusLifetime ServiceLifetime { get; set; } = EBusLifetime.Transient;
    public uint Version { get; set; } = 1;
    protected BusKeyAttribute(string prefix, string plugin, string action) : this($"{prefix}.{plugin}.{action}") { }
    protected BusKeyAttribute(string key) : this(new MessageKey(key)) { }
    protected BusKeyAttribute(MessageKey key)
    {
        Key = key;
    }
}
