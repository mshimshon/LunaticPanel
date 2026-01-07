using LunaticPanel.Engine.Application.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Text.Json;
using static LunaticPanel.Engine.Web.Boostrap.BootstrapPlugins;
using static LunaticPanel.Engine.Web.Boostrap.BootstrapPluginsValidator;
namespace LunaticPanel.Engine.Web.Boostrap;

public static class Bootstrap
{
    private const string ConfigNameKey = "LunaticPanel";

    public static string PluginDirectory { get; private set; } = default!;
    public static string ConfigDirectory { get; private set; } = default!;
    internal static BootstrapConfiguration Configuration { get; private set; } = new();

    public static List<Assembly> AdditionalAssemblies => [.. Configuration.ActivePlugins.Select(p => p.EntryPoint!.GetType().Assembly!)];
    public static UnixFileMode DefaultDirectoryPermissions { get; } =
        UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
        UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
        UnixFileMode.OtherRead | UnixFileMode.OtherExecute;

    public static void BootstrapBuilder(IServiceCollection services, IConfiguration configuration)
    {


        // ORDER MATTERS, IT AFFECTS PLUGIN DISABLING CAPABILITIES DURING BOOTUP.
        DefinePath(configuration);
        DetectPlugins();
        LoadConfiguration();


        services.AddLunaticPanelServices();
        services.ProcessPlugins(configuration);
        EnsurePluginValidatedBlazor();
        SaveConfiguration();
    }

    public static void BootstrapRun(WebApplication webApp, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var pluginRegistry = serviceProvider.GetRequiredService<IPluginRegistry>();
        foreach (BootstrapPluginDescriptor item in Configuration.ActivePlugins)
        {
            pluginRegistry.Register(new(item.EntryPoint!, item.Entity));

            var wwwroot = Path.Combine(item.PluginDir, "wwwroot");
            if (Directory.Exists(wwwroot))
            {
                Console.WriteLine("PLUGIN STATIC FILE REGISTRATION");
                Console.WriteLine($"PackageId: {item.Entity.Identity.PackageId}");
                Console.WriteLine($"PluginDir: {item.PluginDir}");
                Console.WriteLine($"wwwroot: {wwwroot}");
                Console.WriteLine($"wwwroot exists: {Directory.Exists(wwwroot)}");
                Console.WriteLine($"banner exists: {File.Exists(Path.Combine(wwwroot, "game_banner.jpg"))}");
                var options = new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(wwwroot),
                    RequestPath = $"/_plugins/{item.Entity.Identity.PackageId}"
                };
                webApp.UseStaticFiles(options);
            }

        }
    }

    private static void LoadConfiguration()
    {
        var configFile = Path.Combine(ConfigDirectory, "bootstrap.json");
        if (!File.Exists(configFile))
            Configuration = new BootstrapConfiguration();
        else
        {
            string configJson = File.ReadAllText(configFile);
            Configuration = JsonSerializer.Deserialize<BootstrapConfiguration>(configJson)!;
        }

    }

    private static void SaveConfiguration()
    {
        var configJson = JsonSerializer.Serialize(Configuration);
        var configFile = Path.Combine(ConfigDirectory, "bootstrap.json");
        File.WriteAllText(configFile, configJson);
    }

    private static void DefinePath(IConfiguration configuration)
    {


        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        PluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        string? configuredConfigPath = configuration.GetSection(ConfigNameKey).GetValue<string>("ConfigDirectory");

        if (configuredConfigPath == default)
            ConfigDirectory = Path.Combine(appDataDir, ConfigNameKey);
        else
            ConfigDirectory = configuredConfigPath;
        EnsurePathCreated(PluginDirectory, ConfigDirectory);

    }

    private static void EnsurePathCreated(params string[] pathList)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        foreach (var p in pathList)
            if (!Directory.Exists(p))
                if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
                    Directory.CreateDirectory(p, DefaultDirectoryPermissions);
                else
                    Directory.CreateDirectory(p);
#pragma warning restore CA1416 // Validate platform compatibility
    }


}
