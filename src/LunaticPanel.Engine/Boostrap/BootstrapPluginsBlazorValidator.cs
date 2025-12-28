using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace LunaticPanel.Engine.Boostrap;

internal static class BootstrapPluginsBlazorValidator
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
            var pageRouteRulesRespected = IsPluginPageNamingValid(plugin.Entity.Identity.PackageId, plugin);
            if (!pageRouteRulesRespected)
                UnloadFailedPlugin(plugin, $"One or more Routes inside {plugin.Entity.Identity.PackageId} doesn't respect naming conventions.");
        }
    }
    private static void UnloadFailedPlugin(BootstrapPluginDescriptor plugin, string message)
    {
        var activePluginItem = Configuration.ActivePlugins.Single(p => p.EntryPointType == plugin.EntryPointType);
        var knownPluginItem = Configuration.KnownPlugins.Single(p => p.EntryPointType == plugin.EntryPointType);
        Configuration.ActivePlugins.Remove(activePluginItem);
        Configuration.KnownPlugins.Remove(knownPluginItem);
        Configuration.KnownPlugins.Add(plugin.FailedToLoadMapping(message));
    }
    private static bool IsPluginPageNamingValid(string pluginId, BootstrapPluginDescriptor plugin)
    {
        var invalidRoutes = new List<string>();

        foreach (var type in plugin.EntryPointType.Assembly.GetTypes())
        {
            if (!typeof(ComponentBase).IsAssignableFrom(type))
                continue;

            var routes = type.GetCustomAttributes<RouteAttribute>(inherit: false);

            foreach (var route in routes)
            {
                var template = route.Template.TrimStart('/');

                if (!template.StartsWith(pluginId, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"ERROR NAMING ROUTE: {type.FullName} → /{template} should be → /{type.FullName}/{template} ");
                    invalidRoutes.Add($"{type.FullName} → /{template}");
                }
            }
        }

        if (invalidRoutes.Count > 0)
            return false;
        return true;
    }
}
