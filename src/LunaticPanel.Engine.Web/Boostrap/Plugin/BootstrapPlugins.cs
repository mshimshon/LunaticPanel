using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Domain.Plugin.Entites;
using LunaticPanel.Engine.Domain.Plugin.Enums;
using LunaticPanel.Engine.Domain.Plugin.ValueObjects;
using LunaticPanel.Engine.Infrastructure.Plugin;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Plugin.Entities;
using Microsoft.Extensions.Configuration;
namespace LunaticPanel.Engine.Web.Boostrap.Plugin;

internal static class BootstrapPlugins
{
    private static List<BootstrapPluginDescriptor> DiscoveredPlugins { get; set; } = new();
    public static BootstrapConfiguration Configuration => Bootstrap.Configuration;
    public static string PluginDirectory => Bootstrap.PluginDirectory;
    public static string ConfigDirectory => Bootstrap.ConfigDirectory;
    public static IPluginRegistry Registry { get; set; } = new PluginRegistry();
    public static void DetectPlugins()
    {
        var result = PluginScannerExt.ScanAndFindPlugins(PluginDirectory, [], DependencySettings.ScanSharedFrameworkNames());
        foreach (PluginScannedEntity item in result)
        {
            var identity = new PluginIdentity(item.PluginId, item.Version, item.PluginId);
            try
            {
                IPlugin plugin = item.CreateEntryPoint();
                var lifecycle = new PluginLifecycle(PluginState.Loaded, PluginStartupState.Disabled, default, DateTimeOffset.UtcNow);
                var entity = new PluginEntity(identity, lifecycle);

                DiscoveredPlugins.Add(new BootstrapPluginDescriptor()
                {
                    Entity = entity,
                    EntryPoint = plugin,
                    Loader = item.Loader,
                    PluginDir = Path.GetDirectoryName(item.Location)!

                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Scanning ({identity.PackageId}) Plugins Exception: {ex.Message}");
                var failure = new PluginFailure(ex.Message, DateTimeOffset.UtcNow);
                var lifecycle = new PluginLifecycle(PluginState.Failed, PluginStartupState.Disabled, failure, DateTimeOffset.UtcNow);
                var entity = new PluginEntity(identity, lifecycle);
                DiscoveredPlugins.Add(new BootstrapPluginDescriptor()
                {
                    Entity = entity,
                    Loader = item.Loader,
                    PluginDir = Path.GetDirectoryName(item.Location)!

                });
            }

        }
    }

    public static void ProcessPlugins(this IServiceCollection services, IConfiguration configuration)
    {
        var knownCopy = Configuration.KnownPlugins.Select(p => p with { }).ToList();
        Configuration.KnownPlugins.Clear();

        foreach (var plugin in DiscoveredPlugins)
        {
            if (plugin.EntryPoint == default) continue;
            var discovered = knownCopy.SingleOrDefault(p => p.Entity.Identity.PackageId == plugin.Entity.Identity.PackageId);

            if (ShouldDisable(discovered))
            {
                AddDisabledPlugin(plugin);
                continue;
            }

            if (HasPriorFailure(discovered!, plugin))
                AddFailedPlugin(plugin, plugin.Entity.Lifecycle.Failure!);

            TryActivatePlugin(plugin, configuration.GetSection(plugin.Entity.Identity.PackageId));
        }
        AddMisingPlugins(knownCopy);
        Configuration.ActivePlugins = Configuration.KnownPlugins.Where(p => p.Entity.Lifecycle.State == PluginState.Active && p.EntryPoint != default).ToList();


    }

    private static bool ShouldDisable(BootstrapPluginDescriptor? discovered) =>
        discovered == default || discovered.Entity.Lifecycle.StartupState == PluginStartupState.Disabled;

    private static bool HasPriorFailure(BootstrapPluginDescriptor discovered, BootstrapPluginDescriptor plugin) =>
        discovered?.Entity.Lifecycle.StartupState == PluginStartupState.Enabled && plugin.Entity.Lifecycle.Failure != default;

    private static void AddDisabledPlugin(BootstrapPluginDescriptor plugin)
    {
        plugin.Loader!.Unload();
        Configuration.KnownPlugins.Add(plugin.DisablePluginMapping());
    }

    private static void AddFailedPlugin(BootstrapPluginDescriptor plugin, PluginFailure failure)
    {
        Configuration.KnownPlugins.Add(
            plugin.FailedToLoadMapping(failure.Message, failure.OccurredAt)
                  .DisablePluginMapping()
        );
    }

    private static void AddActivatedPlugin(BootstrapPluginDescriptor plugin)
    {
        Configuration.KnownPlugins.Add(plugin.ActivatedPluginMapping().EnablePluginMapping());

    }

    private static void TryActivatePlugin(BootstrapPluginDescriptor plugin, IConfiguration configuration)
    {
        try
        {
            Console.WriteLine($"{plugin.Entity.Identity.PackageId} is trying to activate.");

            plugin.EntryPoint!.Configure(configuration);
            AddActivatedPlugin(plugin);
            Console.WriteLine($"{plugin.Entity.Identity.PackageId} activated");
        }
        catch (Exception ex)
        {
            Console.WriteLine("===== FAILED PLUGIN ACTIVATION ====");
            Console.WriteLine(plugin.Entity.Identity.PackageId);
            Console.WriteLine(ex.Message);
            AddFailedPlugin(plugin, new(ex.Message, DateTimeOffset.UtcNow));
        }
    }

    private static void AddMisingPlugins(List<BootstrapPluginDescriptor> oldPlugins)
    {
        foreach (var oldPlugin in oldPlugins)
        {
            if (Configuration.KnownPlugins.Any(p => p.Entity.Identity.PackageId == oldPlugin.Entity.Identity.PackageId))
                continue;
            Configuration.KnownPlugins.Add(oldPlugin.MissingPluginMapping().DisablePluginMapping());
        }
    }

}
