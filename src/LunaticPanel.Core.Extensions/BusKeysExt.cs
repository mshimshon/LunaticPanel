using LunaticPanel.Core.Abstraction.Messaging.Extensions;
using System.Reflection;

namespace LunaticPanel.Core.Extensions;

public static class BusKeysExt
{

    public static string[] ScanKeyPackageForKeys(this Assembly keyAssembly)
        => Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.IsClass || type.IsValueType)
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(field => field.IsLiteral)
            .Where(field => field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!)
            .Where(MessageKeyValidator.ValidateKeyPattern)
            .ToArray();
}
