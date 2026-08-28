using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.PluginValidator;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Domain.Plugin.Entites;
using LunaticPanel.Engine.Domain.Plugin.Enums;
using LunaticPanel.Engine.Domain.Plugin.ValueObjects;
using LunaticPanel.Engine.Infrastructure.Plugin;
using LunaticPanel.Engine.Plugin;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
namespace LunaticPanel.Engine.Web.Boostrap.Plugin;

internal static class BootstrapPlugins
{
    private static List<BootstrapPluginDescriptor> DiscoveredPlugins { get; set; } = new();
    private static Dictionary<string, BootstrapPluginManifest> PreInstalled { get; set; } = new();
    private static Dictionary<string, BootstrapPluginManifest> Runtimes { get; set; } = new();
    private static Dictionary<string, BootstrapPluginManifest> ToApplyUpdates { get; set; } = new();
    private static Dictionary<string, BootstrapPluginManifest> Installed { get; set; } = new();
    private static Dictionary<string, BootstrapPluginManifest> Rollbacks { get; set; } = new();
    public static BootstrapConfiguration Configuration => Bootstrap.Configuration;
    public static string PluginDirectory => Bootstrap.PluginDirectory;
    public static string ConfigDirectory => Bootstrap.ConfigDirectory;
    public static IPluginRegistry Registry { get; set; } = new PluginRegistry();
    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };
    /// <summary>
    /// if plugin folder does not contain preinstall copy to apply updates.
    /// </summary>
    public static void ProcessPreinstalledPlugins()
    {
        DetectUpdates();
        string preinstall = Path.Combine(Environment.CurrentDirectory, "plugins_preinstalled");
        if (!Directory.Exists(preinstall)) return;

        string applyLocation = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins", "apply");
        string[] packages = Directory.GetFiles(preinstall, "*.lpkg", SearchOption.TopDirectoryOnly);
        foreach (var item in packages)
        {
            var manifest = ReadManifestFromPackage(item);
            if (PreInstalled.Values.Any(p => p.Id == manifest.Id))
                continue;
            PreInstalled[item] = manifest;
            if (ToApplyUpdates.Values.Any(p => p.Id == manifest.Id))
                continue;
            string linuxPluginName = manifest.Id.ToLower().Replace('.', '_');
            string runtimeLocation = Path.Combine(Environment.CurrentDirectory, "plugins", linuxPluginName);
            if (Directory.Exists(runtimeLocation)) continue;
            File.Copy(item, applyLocation);
        }
        DetectUpdates();
    }

    /// <summary>
    /// Read Folder Apply and unpack to plugins/ then move lpkg into rollback if in installed and move apply to installed.
    /// </summary>
    public static void DetectUpdates()
    {
        ToApplyUpdates.Clear();
        string location = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins", "apply");
        string[] packages = Directory.GetFiles(location, "*.lpkg", SearchOption.TopDirectoryOnly);
        foreach (var item in packages)
        {
            var manifest = ReadManifestFromPackage(item);
            if (ToApplyUpdates.Values.Any(p => p.Id == manifest.Id))
                continue;
            ToApplyUpdates[item] = manifest;
        }
    }

    public static void ApplyUpdates()
    {
        string applyLocation = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins", "apply");
        string rollbacksLocation = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins", "rollbacks");
        string installedLocation = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins", "installed");
        var asm = typeof(IPlugin).Assembly;
        var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion.Split('-', '+')[0];
        Version currentPanelVersion = new(info);
        // Check for Package Id in Installed
        // Check for Package Id in Rollbacks
        // Check for Package Version in Installed
        // If Versions are different, remove existing package from rollback, move the installed into to rollback.
        // Move Package from apply to install.
        // Clear existing runtime plugin folder, extract package to same directory.
        foreach (var toApply in ToApplyUpdates)
        {
            var rollback = Rollbacks.FirstOrDefault(p => p.Value.Id == toApply.Value.Id);
            var installed = Installed.FirstOrDefault(p => p.Value.Id == toApply.Value.Id);
            var hasRollback = Rollbacks.Any(p => p.Value.Id == toApply.Value.Id);
            var hasInstalled = Installed.Any(p => p.Value.Id == toApply.Value.Id);
            Version pluginPanelVersion = new Version(toApply.Value.PanelVersion);
            if (pluginPanelVersion > currentPanelVersion)
            {
                Console.Error.WriteLine("The plugin was compiled using more recent panel version therefore not compatible. (Removing)");
                File.Delete(toApply.Key);
                continue;
            }
            if (pluginPanelVersion.Major != currentPanelVersion.Major)
            {
                Console.Error.WriteLine("The plugin was compiled using more different major panel version, therefore not compatible. (Removing)");
                File.Delete(toApply.Key);
                continue;
            }
            string linuxPluginName = toApply.Value.Id.ToLower().Replace('.', '_');
            string runtimeLocation = Path.Combine(Environment.CurrentDirectory, "plugins", linuxPluginName);
            if (Directory.Exists(runtimeLocation))
                Directory.Delete(runtimeLocation, true);

            Directory.CreateDirectory(runtimeLocation);
            ExtractZipToRoot(toApply.Key, runtimeLocation);
            if (hasRollback)
                File.Delete(rollback.Key);

            if (hasInstalled)
                File.Move(installed.Key, Path.Combine(rollbacksLocation, Path.GetFileName(installed.Key)));

            File.Move(toApply.Key, Path.Combine(installedLocation, Path.GetFileName(toApply.Key)));

        }
    }

    public static void DetectRollbacks()
    {
        Rollbacks.Clear();
        string location = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins", "rollbacks");
        string[] packages = Directory.GetFiles(location, "*.lpkg", SearchOption.TopDirectoryOnly);
        foreach (var item in packages)
        {
            var manifest = ReadManifestFromPackage(item);
            if (Rollbacks.Values.Any(p => p.Id == manifest.Id))
                continue;
            Rollbacks[item] = manifest;
        }
    }

    public static void DetectInstalled()
    {
        Installed.Clear();
        string location = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins", "installed");
        string[] packages = Directory.GetFiles(location, "*.lpkg", SearchOption.TopDirectoryOnly);
        foreach (var item in packages)
        {
            var manifest = ReadManifestFromPackage(item);
            if (Installed.Values.Any(p => p.Id == manifest.Id))
                continue;
            Installed[item] = manifest;
        }
    }

    public static BootstrapPluginManifest ReadManifestFromPackage(string path)
    {
        using var archive = ZipFile.OpenRead(path);

        var entry = archive.GetEntry("manifest.json");
        if (entry == null)
            throw new FileNotFoundException($"manifest.json not found in {path}");

        using var stream = entry.Open();
        using var reader = new StreamReader(stream);
        var result = JsonSerializer.Deserialize<BootstrapPluginManifest>(reader.ReadToEnd(), _jsonSerializerOptions);
        if (result == null)
            throw new FileNotFoundException($"manifest.json corrupted in {path}");
        return result;
    }


    private static void ExtractZipToRoot(string zipPath, string targetRoot)
    {
        Directory.CreateDirectory(targetRoot);

        using var zip = ZipFile.OpenRead(zipPath);

        foreach (var entry in zip.Entries)
        {
            // Skip directory entries
            if (string.IsNullOrEmpty(entry.Name))
                continue;

            string outputPath = Path.Combine(targetRoot, entry.FullName);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            using var input = entry.Open();
            using var output = File.Create(outputPath);
            input.CopyTo(output);
        }
    }

    public static void DetectRuntimePlugins()
    {
        var result = PluginScannerExt.ScanAndFindPlugins(PluginDirectory, [], DependencySettings.ScanSharedFrameworkNames());
        Runtimes.Clear();
        foreach (var dll in result)
        {
            if (!LibraryValidatorExt.IsPluginDllValid(dll))
            {
                Console.WriteLine($"Failed to load '{dll}', skipping.");
                continue;
            }

            var metadata = LibraryValidatorExt.ExtractMetadata(dll);
            if (metadata == default)
            {
                Console.WriteLine($"Failed to extract metadata for '{dll}', skipping.");
                continue;
            }
            if (Runtimes.Values.Any(p => p.Id == metadata[ManifestMeta.Id]))
            {
                Console.WriteLine($"Duplicate Plugin for '{metadata[ManifestMeta.Id]}', skipping.");
                continue;
            }


            var path = Path.Combine(Path.GetDirectoryName(dll)!, "manifest.json");
            var manifest = JsonSerializer.Deserialize<BootstrapPluginManifest>(File.ReadAllText(path), _jsonSerializerOptions);
            if (manifest == default)
            {
                Console.WriteLine($"Failed to extract Manifest for '{dll}', skipping.");
                continue;
            }
            if (metadata[ManifestMeta.Id] == default)
            {
                Console.WriteLine($"Plugin ID missing for '{dll}', skipping.");
                continue;
            }
            if (metadata[ManifestMeta.Description] == default)
            {
                Console.WriteLine($"Plugin description missing for '{dll}', skipping.");
                continue;
            }

            if (metadata[ManifestMeta.Version] == default)
            {
                Console.WriteLine($"Plugin Version missing for '{dll}', skipping.");
                continue;
            }

            if (metadata[ManifestMeta.FileVersion] == default)
            {
                Console.WriteLine($"Plugin FileVersion missing for '{dll}', skipping.");
                continue;
            }

            if (metadata[ManifestMeta.AssemblyVersion] == default)
            {
                Console.WriteLine($"Plugin AssemblyVersion missing for '{dll}', skipping.");
                continue;
            }

            if (metadata[ManifestMeta.Id] != manifest.Id)
            {
                Console.WriteLine($"Plugin ID missing for '{dll}', skipping.");
                continue;
            }
            if (metadata[ManifestMeta.Description] == default)
            {
                Console.WriteLine($"Plugin description missing for '{dll}', skipping.");
                continue;
            }

            if (metadata[ManifestMeta.Version]!.Split('-', '+')[0] != manifest.Version)
            {
                Console.WriteLine($"Plugin Version corrupted for '{dll}', skipping.");
                continue;
            }

            if (metadata[ManifestMeta.FileVersion] != manifest.Version)
            {
                Console.WriteLine($"Plugin FileVersion corrupted for '{dll}', skipping.");
                continue;
            }

            if (metadata[ManifestMeta.AssemblyVersion] != manifest.Version)
            {
                Console.WriteLine($"Plugin AssemblyVersion corrupted for '{dll}', skipping.");
                continue;
            }

            Runtimes[dll] = manifest;
        }
    }

    public static void DetectPlugins()
    {
        foreach (var dll in Runtimes)
        {
            if (!LibraryValidatorExt.IsPluginDllValid(dll.Key))
            {
                Console.WriteLine($"Failed to load '{dll}', skipping.");
                continue;
            }

            var item = PluginScannerExt.LoadPluginInformation(dll.Key, [], DependencySettings.ScanSharedFrameworkNames());
            if (item == default)
            {
                Console.WriteLine($"Failed to load '{dll}', skipping.");
                continue;
            }
            var identity = new PluginIdentity(item.PluginId, item.Version, item.PluginId);
            try
            {
                IPlugin plugin = item.CreateEntryPoint();
                // Auto Enable Plugin if PreInstalled.
                PluginStartupState dectectionState = PreInstalled.Any(p => p.Value.Id == item.PluginId) ?
                    PluginStartupState.Enabled :
                    PluginStartupState.Disabled;
                var lifecycle = new PluginLifecycle(PluginState.Loaded, dectectionState, default, DateTimeOffset.UtcNow);
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
