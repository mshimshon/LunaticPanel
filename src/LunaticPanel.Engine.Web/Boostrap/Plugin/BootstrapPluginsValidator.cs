namespace LunaticPanel.Engine.Web.Boostrap.Plugin;

internal static class BootstrapPluginsValidator
{
    public static BootstrapConfiguration Configuration => Bootstrap.Configuration;
    public static string PluginDirectory => Bootstrap.PluginDirectory;
    public static string ConfigDirectory => Bootstrap.ConfigDirectory;
    public static void EnsurePluginValidatedBlazor()
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S1215:Do not call GC.Collect()",
    Justification = "Required for unloading collectible AssemblyLoadContext")]

    internal static void GarbageCleanUp()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
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
        plugin.Loader!.Dispose();
        GarbageCleanUp();

    }

}
