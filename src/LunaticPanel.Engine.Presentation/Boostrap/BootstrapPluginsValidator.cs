using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Core.PluginValidator;
using LunaticPanel.Core.PluginValidator.Diagnostic.Messages;

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
        {
            ValidationResult result = plugin.EntryPoint!.FindAnyInvalidRoutesNames();
            if (!result.Passed)
                UnloadFailedPlugin(plugin, $"One or more Routes inside {plugin.Entity.Identity.PackageId} doesn't respect naming conventions.");

            result = plugin.EntryPoint!.FindAnyWidgetNotUsingProperComponentBase();
            if (!result.Passed)
                UnloadFailedPlugin(plugin, $"One or more Components inside {plugin.Entity.Identity.PackageId} doesn't respect {nameof(WidgetComponentBase<,>)} inheritance requirement.");
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
