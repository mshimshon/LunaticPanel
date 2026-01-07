namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public sealed record MessageKey
{
    private readonly string _fullname;

    public MessageKey(string fullname)
    {
        _fullname = fullname;
    }

    public MessageKey(string plugin, string action) : this($"{plugin}.{action}") { }
    public MessageKey(Func<string> plugin, string action) : this(plugin(), action) { }
    public override string ToString() => _fullname;
}