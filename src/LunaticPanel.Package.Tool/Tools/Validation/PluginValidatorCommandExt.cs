using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Plugin.Validation;
using LunaticPanel.Package.Tool.Exceptions;

namespace LunaticPanel.Package.Tool.Tools.Validation;

internal static class PluginValidatorCommandExt
{
    public static async Task<bool> ValidatePackageAsync(string input)
    {
        var entity = PluginScannerExt.FindPluginDllInDirectory(input);
        if (entity == default)
            throw new PluginDllNotFoundException(input);

        // No Plugins can dependent on other plugins directly.
        var hasValidDependencyRule = DependenciesValidationExt.ValidateNoHardDependencies(input);
        if (!hasValidDependencyRule)
            throw new PluginHardDependencyViolationException(input);

        var pluginAsm = entity.Loader.Load();
        var plugin = entity.CreateEntryPoint(pluginAsm);

        IReadOnlyCollection<Core.Abstraction.Diagnostic.Messages.PluginValidationResult>? pluginValidation = plugin.PerformValidation();
        if (pluginValidation.Any(p => !p.Passed))
        {
            var error = pluginValidation.First(p => !p.Passed).Errors!.First();
            throw new PluginValidationFailedException(plugin.PluginId, error);
        }
        return false;
    }
}
