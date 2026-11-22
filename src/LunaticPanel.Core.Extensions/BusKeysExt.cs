using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.Extensions;
using System.Reflection;

namespace LunaticPanel.Core.Extensions;

public static class BusKeysExt
{

    public static MessageKey[] ScanKeyPackageForKeys(this Assembly keyAssembly)
        => keyAssembly.GetTypes()
        .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
        .Where(f => f.FieldType == typeof(string))
        .Where(f => f.IsLiteral || f.IsInitOnly) // const OR static readonly
        .Select(f => (string)f.GetValue(null)!)
        .Where(MessageKeyValidator.ValidateKeyPattern)
        .Select(MessageKey.Create)
        .ToArray();
}
