using LunaticPanel.Core.PluginValidator;

namespace LunaticPanel.Engine.Plugin.Validation;

public static class DependenciesValidationExt
{
    public static bool ValidateNoHardDependencies(string pluginDir)
    {
        int count = 0;
        foreach (var dll in Directory.GetFiles(pluginDir, "*.dll", SearchOption.TopDirectoryOnly))
        {
            if (count > 1) return false;
            count += LibraryValidatorExt.CountIPluginImplementations(dll);
        }
        return true;
    }

    public static bool ValidateNoIPluginDuplicates(string dll)
    {
        var count = LibraryValidatorExt.CountIPluginImplementations(dll);
        return count <= 1;
    }

    internal static int CountHardPluginDependencies(string pluginDir)
    {
        int count = 0;
        foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
        {

            var entity = PluginScannerExt.LoadPluginInformation(dll, [], []);

            if (entity == default) continue;
            count++;
        }
        return count;
    }
}
