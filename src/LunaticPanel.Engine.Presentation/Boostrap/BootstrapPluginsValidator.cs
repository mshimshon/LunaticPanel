namespace LunaticPanel.Engine.Web.Boostrap;

internal static class BootstrapPluginsValidator
{
    public static BootstrapConfiguration Configuration => Bootstrap.Configuration;
    public static string PluginDirectory => Bootstrap.PluginDirectory;
    public static string ConfigDirectory => Bootstrap.ConfigDirectory;
    public static void EnsurePluginValidatedBlazor()
    {
        RemoveViolatingPluginsRoutes();
    }
    private static void RemoveViolatingPluginsRoutes()
    {
        var cachePLugin = Configuration.ActivePlugins.Select(p => p with { }).ToList();
        foreach (var plugin in cachePLugin)
            foreach (var result in plugin.EntryPoint!.PerformValidation())
            {
                if (!result.Passed)
                {
                    UnloadFailedPlugin(plugin, result.Errors!.First().Message);
                    break;
                }

            }
    }
    private static void UnloadFailedPlugin(BootstrapPluginDescriptor plugin, string message)
    {

        var activePluginItem = Configuration.ActivePlugins
            .Single(p => p.Entity.Identity.PackageId == plugin.Entity.Identity.PackageId);
        var knownPluginItem = Configuration.KnownPlugins
            .Single(p => p.Entity.Identity.PackageId == plugin.Entity.Identity.PackageId);
        Configuration.ActivePlugins.Remove(activePluginItem);
        Configuration.KnownPlugins.Remove(knownPluginItem);
        Configuration.KnownPlugins.Add(plugin.FailedToLoadMapping(message));
    }

}
