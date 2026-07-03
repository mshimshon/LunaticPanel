namespace LunaticPanel.Engine.Plugin.Validation;

public static class DependenciesValidationExt
{
    public static bool ValidateNoHardDependencies(string pluginDir)
    {
        var count = CountHardPluginDependencies(pluginDir);
        return count <= 1;
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
