using LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.Extensions;

namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public sealed record MessageKey
{
    private readonly string _fullname;
    private readonly string[] _allowedPrefixes = [];
    public MessageKey(string fullname)
    {
        _fullname = fullname.ToLower();
        if (!MessageKeyValidator.ValidateAllowedCharacters(fullname))
            throw new BusKeySchemticAllowedCharactersViolationException(fullname);
        if (!MessageKeyValidator.ValidatePrefix(fullname))
            throw new BusKeySchemticPrefixViolationException(fullname);
        if (!MessageKeyValidator.ValidateKeyPattern(fullname))
            throw new BusKeySchematicPatternViolationException(fullname);
    }

    public MessageKey(string prefix, string plugin, string action) : this($"{prefix}.[{plugin}].{action}") { }
    public MessageKey(string prefix, string plugin, string version, string action) : this(prefix, plugin, $"v{version}.{action}") { }
    public MessageKey(string prefix, Func<string> plugin, string version, string action) : this(prefix, plugin(), version, action) { }
    public MessageKey(string prefix, Func<string> plugin, string action) : this(prefix, plugin(), action) { }
    public override string ToString() => _fullname;
}