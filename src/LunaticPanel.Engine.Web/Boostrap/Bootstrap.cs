using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Web.Boostrap.Plugin;
using LunaticPanel.Engine.Web.Services.Circuit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Text.Json;
using static LunaticPanel.Engine.Web.Boostrap.Plugin.BootstrapPlugins;
using static LunaticPanel.Engine.Web.Boostrap.Plugin.BootstrapPluginsValidator;
namespace LunaticPanel.Engine.Web.Boostrap;

public static class Bootstrap
{
    private const string ConfigNameKey = "lunaticpanel";

    public static string PluginDirectory { get; private set; } = default!;
    public static string ConfigDirectory { get; private set; } = default!;
    internal static BootstrapConfiguration Configuration { get; private set; } = new();
    public static string LibraryLocation { get; set; } = default!;
    public static string ConfigLocation { get; set; } = default!;
    public static List<Assembly> AdditionalAssemblies => [.. Configuration.ActivePlugins.Select(p => p.EntryPoint!.GetType().Assembly!), typeof(RegisterServicesExt).Assembly];
    public static UnixFileMode DefaultDirectoryPermissions { get; } =
        UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
        UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
        UnixFileMode.OtherRead | UnixFileMode.OtherExecute;

    public static void BootstrapBuilder(IServiceCollection services, IConfiguration configuration)
    {
        LibraryLocation = "/usr/lib";
        ConfigLocation = "/etc";

        // ORDER MATTERS, IT AFFECTS PLUGIN DISABLING CAPABILITIES DURING BOOTUP.
        DefinePath(configuration);
        DetectPlugins();
        LoadConfiguration();


        services.AddLunaticPanelServices();
        services.ProcessPlugins(configuration);
        EnsurePluginValidatedBlazor();
        SaveConfiguration();


    }

    public static async Task BootstrapRunAsync(WebApplication? webApp, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var masterSp = serviceProvider.CreateScope().ServiceProvider;
        var pluginRegistry = masterSp.GetRequiredService<IPluginRegistry>();
        var circuitRegistry = masterSp.GetRequiredService<CircuitRegistry>();
        foreach (BootstrapPluginDescriptor plugin in Configuration.ActivePlugins)
        {
            pluginRegistry.Register(new(plugin.EntryPoint!, plugin.Entity));
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("===== LOADED PLUGIN =====");
            Console.WriteLine($"PackageId: {plugin.Entity.Identity.PackageId}");
            Console.WriteLine($"PluginDir: {plugin.PluginDir}");
            Console.WriteLine("===== END PLUGIN =====");
            Console.ForegroundColor = ConsoleColor.Gray;

            var wwwroot = Path.Combine(plugin.PluginDir, "wwwroot");
            if (Directory.Exists(wwwroot))
            {
                Console.WriteLine($"wwwroot ({Directory.Exists(wwwroot)}): {wwwroot}");
                var options = new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(wwwroot),
                    RequestPath = $"/_plugins/{plugin.Entity.Identity.PackageId}"
                };
                if (webApp != default)
                    webApp.UseStaticFiles(options);
            }
            var redirectServiceToHost = RegisterServicesExt
                .AddHostRedirectedServices(new ServiceCollection())
                .Select(p => new HostRedirectionService(p.ServiceType, p.Lifetime))
                .ToArray();
            plugin.EntryPoint!.AddHostRedirectedServices(redirectServiceToHost);
        }

        circuitRegistry.SelfCircuitRegistration(Guid.NewGuid(), masterSp, default);
        foreach (BootstrapPluginDescriptor plugin in Configuration.ActivePlugins)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("===== INITIALIZING PLUGIN =====");
            Console.WriteLine($"PackageId: {plugin.Entity.Identity.PackageId}");
            Console.WriteLine($"PluginDir: {plugin.PluginDir}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            await plugin.EntryPoint!.BeforeRuntimeStartAsync(masterSp);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("===== PLUGIN INITIALIZED =====");
            Console.ForegroundColor = ConsoleColor.Gray;
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
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var configJson = JsonSerializer.Serialize(Configuration, options);
        var configFile = Path.Combine(ConfigDirectory, "bootstrap.json");
        File.WriteAllText(configFile, configJson);
    }

    private static void DefinePath(IConfiguration configuration)
    {


        PluginDirectory = Path.Combine(LibraryLocation, ConfigNameKey, "plugins");
        ConfigDirectory = Path.Combine(ConfigLocation, ConfigNameKey);
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
