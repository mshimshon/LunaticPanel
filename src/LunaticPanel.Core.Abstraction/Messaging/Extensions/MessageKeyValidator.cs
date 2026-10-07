using System.Text.RegularExpressions;

namespace LunaticPanel.Core.Abstraction.Messaging.Extensions;

public static class MessageKeyValidator
{
    //private const string REG_PATTERN_VALIDATION = @"(enginekey|querykey|eventkey)\.[a-zA-Z0-9_-]+(?:\.[a-zA-Z0-9_-]+)\.v[1-9][0-9]\.[a-zA-Z0-9_-]+(?:\.[a-zA-Z0-9_-]+)*$";
    //private const string REG_ASSEMBLY_VALIDATION = @"^(enginekey|querykey|eventkey)\.{0}\.v[1-9][0-9]*\.[a-zA-Z0-9_-]+(?:\.[a-zA-Z0-9_-]+)*$";
    //private const string REG_VALID_CHARS = @"^[a-zA-Z0-9._]+$";
    //private const string REG_VALID_PREFIX = @"^(enginekey|querykey|eventkey)\.";
    // 1. REG_PATTERN_VALIDATION
    // Enforces: prefix.[discriminator].version.tail
    // - Explicitly requires '[' and ']' around the discriminator.
    // - Allows alphanumeric, '_', '-' inside the segments.
    private const string REG_PATTERN_VALIDATION =
        @"^(enginekey|querykey|eventkey)\.\[([a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)*)\]\.v[1-9][0-9]*\.[a-zA-Z0-9_-]+(?:\.[a-zA-Z0-9_-]+)*$";

    // The {0} is now strictly wrapped in literal brackets [ and ]
    private const string REG_ASSEMBLY_VALIDATION = @"^(enginekey|querykey|eventkey)\.\[{0}\]\.v[1-9][0-9]*\.[a-zA-Z0-9_]+(?:\.[a-zA-Z0-9_]+)*$";

    // 2. REG_VALID_CHARS
    // NOW INCLUDES '[' and ']' as valid characters.
    // Use this if you validate individual parts of the ID separately.
    private const string REG_VALID_CHARS = @"^[a-zA-Z0-9._\[\]-]+$";

    // 3. REG_VALID_PREFIX
    // Unchanged.
    private const string REG_VALID_PREFIX = @"^(enginekey|querykey|eventkey)\.";
    public static bool ValidatePrefix(string key)
        => Regex.IsMatch(key, REG_VALID_PREFIX, RegexOptions.IgnoreCase);

    public static bool ValidateAssembly(string key, string assembly)
        => Regex.IsMatch(key, string.Format(REG_ASSEMBLY_VALIDATION, assembly), RegexOptions.IgnoreCase);
    public static bool ValidateAllowedCharacters(string key)
        => Regex.IsMatch(key, REG_VALID_CHARS, RegexOptions.IgnoreCase);
    public static bool ValidateKeyPattern(string key)
    {
        return ValidateAllowedCharacters(key) && Regex.IsMatch(key, REG_PATTERN_VALIDATION, RegexOptions.IgnoreCase);
    }
}
