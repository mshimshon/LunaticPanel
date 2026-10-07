using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using LunaticPanel.Core.Abstraction.Plugin;
using System.Reflection;

namespace LunaticPanel.Core.PluginValidator;

public static class CoreValidatorExt
{
    private static readonly string[] CoreAssemblyNames =
    [
        "LunaticPanel.Core",
        "LunaticPanel.Core.Abstraction",
        "LunaticPanel.Core.Extensions",
        "LunaticPanel.Core.Utils",
        "LunaticPanel.Core.Utils.Abstraction"
    ];
    public static PluginValidationResult ValidateCoreAssemblies(this IPlugin plugin)
    {
        List<PluginValidationError> validationErrors = new();

        // 1. Inspect ONLY the plugin assembly's metadata references
        Assembly pluginAssembly = plugin.GetType().Assembly;

        var pluginCoreReferences = pluginAssembly.GetReferencedAssemblies()
            .Where(r => CoreAssemblyNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // 2. Validate only if the plugin targets multiple core assemblies
        if (pluginCoreReferences.Count > 1)
        {
            // Establish the baseline reference from the first core assembly found
            var baselineRef = pluginCoreReferences[0];
            Version baselineVersion = baselineRef.Version!;

            foreach (var reference in pluginCoreReferences.Skip(1))
            {
                Version targetVersion = reference.Version!;
                bool isViolation = false;
                string policyType = string.Empty;

                // RULE 1: Alpha Policy (Major version is 0) -> Must be an exact match
                if (baselineVersion.Major == 0 || targetVersion.Major == 0)
                {
                    policyType = "Alpha Policy (0.y.z)";
                    if (baselineVersion != targetVersion)
                    {
                        isViolation = true;
                    }
                }
                // RULE 2: Production Policy (Major version >= 1) -> Only Major version must match
                else
                {
                    policyType = "Production Policy (x.y.z)";
                    if (baselineVersion < targetVersion)
                    {
                        isViolation = true;
                    }
                }

                // If a policy violation occurs, log it and add it to the validation errors
                if (isViolation)
                {
                    string errorMessage = $"{policyType} Mismatch! Plugin mixed {baselineRef.Name} (v{baselineVersion}) " +
                                         $"with {reference.Name} (v{targetVersion}). This breaks signature safety guidelines.";

                    Console.WriteLine($"ERROR CORE POLICY: {errorMessage}");

                    validationErrors.Add(new()
                    {
                        Message = errorMessage,
                        Origin = pluginAssembly.GetName().Name ?? "UnknownPluginAssembly"
                    });
                }
            }
        }

        // 3. Return using your exact structural pattern
        return new PluginValidationResult
        {
            Errors = validationErrors.AsReadOnly(),
            PluginId = plugin.PluginId
        };
    }
}
