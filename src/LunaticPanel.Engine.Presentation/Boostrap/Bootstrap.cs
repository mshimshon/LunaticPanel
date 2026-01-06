using LunaticPanel.Core;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Infrastructure.Plugin.DependencyController;
using LunaticPanel.Engine.Presentation.Services.Messaging;
using LunaticPanel.Engine.Presentation.Services.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Text.Json;
using static LunaticPanel.Engine.Presentation.Boostrap.BootstrapPlugins;
using static LunaticPanel.Engine.Presentation.Boostrap.BootstrapPluginsBlazorValidator;
namespace LunaticPanel.Engine.Presentation.Boostrap;

public static class Bootstrap
{
    private const string ConfigNameKey = "LunaticPanel";

    public static string PluginDirectory { get; private set; } = default!;
    public static string ConfigDirectory { get; private set; } = default!;
    internal static BootstrapConfiguration Configuration { get; private set; } = new();

    public static List<Assembly> AdditionalAssemblies => [.. Configuration.ActivePlugins.Select(p => p.EntryPointType.Assembly!)];
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
        BootstrapPluginDescriptor[] activePluginsEntryPoint = Configuration.ActivePlugins.ToArray();
        services.ScanAndAddBusHandlersFor(activePluginsEntryPoint);
        services.AddPluginServices(activePluginsEntryPoint);
        SaveConfiguration();
    }

    public static void BootstrapRun(WebApplication webApp, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var pluginRegistry = serviceProvider.GetRequiredService<IPluginRegistry>();
        foreach (BootstrapPluginDescriptor item in Configuration.ActivePlugins)
        {
            pluginRegistry.Register(new(item.EntryPointType, item.EntryPoint!, item.Entity));

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
        serviceProvider.RegisterScannedBusHandlers();

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
        string? configuredPluginPath = configuration.GetSection(ConfigNameKey).GetValue<string>("PluginDirectory");
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (configuredPluginPath == default)
            PluginDirectory = Path.Combine(appDataDir, ConfigNameKey, "Plugins");
        else
            PluginDirectory = configuredPluginPath;

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


    private static void ScanAndAddBusHandlersFor(this IServiceCollection services, params BootstrapPluginDescriptor[] plugins)
    {
        foreach (var plugin in plugins)
        {
            ServiceCollection localBusHandlerCollection = new();
            foreach (var srv in services.ScanBusHandlers(plugin.EntryPoint))
                localBusHandlerCollection.AddTransient(srv.HandlerType);
            PluginDependencyInjectionController.AddPluginServices(plugin.EntryPointType, localBusHandlerCollection.ToList());
        }


    }
    private static void AddPluginServices(this IServiceCollection services, params BootstrapPluginDescriptor[] plugins)
    {
        foreach (var item in plugins)
        {
            var serviceType = typeof(IPluginService<>).MakeGenericType(item.EntryPointType);
            var resolverType = typeof(PluginServiceResolver<>).MakeGenericType(item.EntryPointType);
            services.AddScoped(serviceType, resolverType);
            var srv = new ServiceCollection();
            item.EntryPoint!.RegisterServices(srv);
            PluginDependencyInjectionController.AddPluginServices(item.EntryPointType, srv.ToList());
            Console.WriteLine("{0} is loaded with {1} services.", item.Entity.Identity.PackageId, srv.Count);

        }
    }
}
