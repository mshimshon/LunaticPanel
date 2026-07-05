using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Plugin.Validation;
using LunaticPanel.Package.Tool.Exceptions;

namespace LunaticPanel.Package.Tool.Tools.Validation;

internal static class PluginValidatorCommandExt
{
    public static async Task ValidatePackageAsync(string input)
    {
        Console.Out.WriteLine($"Validating Plugin for {input}");
        var entity = PluginScannerExt.FindPluginDllInDirectory(input);
        if (entity == default)
            throw new PluginDllNotFoundException(input);
        Console.Out.WriteLine($"We Found {entity.PluginId}".Green());

        // No Plugins can dependent on other plugins directly.
        Console.Out.WriteLine($"Checking Hard Dependencies for {entity.PluginId}".Blue());

        var hasValidDependencyRule = DependenciesValidationExt.ValidateNoHardDependencies(input);
        if (!hasValidDependencyRule)
            throw new PluginHardDependencyViolationException(input);
        Console.Out.WriteLine($"Test Plugin Loader for {entity.PluginId}".Blue());

        var pluginAsm = entity.Loader.Load();
        Console.Out.WriteLine($"Creating Entry Point for {entity.PluginId}".Blue());
        var plugin = entity.CreateEntryPoint(pluginAsm);
        Console.Out.WriteLine($"Performing Internal Validation for {entity.PluginId}".Blue());

        IReadOnlyCollection<Core.Abstraction.Diagnostic.Messages.PluginValidationResult>? pluginValidation = plugin.PerformValidation();
        if (pluginValidation.Any(p => p.Errors?.Count > 0))
        {
            var error = pluginValidation.First(p => !p.Passed).Errors!.First();
            Console.Error.WriteLine($"Validation failed for {entity.PluginId} with {error.Message} ".Red());

            throw new PluginValidationFailedException(plugin.PluginId, error);
        }
    }
}
