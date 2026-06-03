namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public abstract class BusIdAttribute : Attribute
{
    public MessageKey Key { get; }
    public EBusLifetime ServiceLifetime { get; set; } = EBusLifetime.Transient;
    public uint Version { get; set; } = 1;
    protected BusIdAttribute(string prefix, string plugin, string action) : this($"{prefix}.{plugin}.{action}") { }
    protected BusIdAttribute(string key) : this(new MessageKey(key)) { }
    protected BusIdAttribute(MessageKey key)
    {
        Key = key;
    }
}
