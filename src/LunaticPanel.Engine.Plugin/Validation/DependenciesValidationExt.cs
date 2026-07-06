using LunaticPanel.Core.Abstraction.Plugin;

namespace LunaticPanel.Engine.Plugin.Validation;

public static class DependenciesValidationExt
{
    public static bool ValidateNoHardDependencies(string pluginDir)
    {
        var count = CountHardPluginDependencies(pluginDir);
        return count <= 1;
    }

    public static bool ValidateNoIPluginDuplicates(string pluginDir)
    {
        var count = CountIPluginDuplicates(pluginDir);
        return count <= 1;
    }
    internal static int CountIPluginDuplicates(string pluginDir)
    {
        int count = 0;
        foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
        {

            var entity = PluginScannerExt.LoadPluginInformation(dll);
            if (entity == default) continue;
            var asm = entity.Loader.Load();
            var iPluginCount = asm.GetTypes().Count(t => t.IsClass && !t.IsAbstract && typeof(IPlugin).IsAssignableFrom(t));
            Console.Out.WriteLine($"{entity.PluginId} has {iPluginCount} IPlugin Implementation.");
            count += iPluginCount;
        }
        return count;
    }
    internal static int CountHardPluginDependencies(string pluginDir)
    {
        int count = 0;
        foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
        {

            var entity = PluginScannerExt.LoadPluginInformation(dll);

            if (entity == default) continue;
            count++;
        }
        return count;
    }
}
