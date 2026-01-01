using LunaticPanel.Core;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Infrastructure.Plugin.DependencyController;
using LunaticPanel.Engine.Presentation.Services.Messaging;
using LunaticPanel.Engine.Presentation.Services.Plugin;
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

    public static WebApplication Load(Func<WebApplicationBuilder> webApplicationBuilder)
    {


        // ORDER MATTERS, IT AFFECTS PLUGIN DISABLING CAPABILITIES DURING BOOTUP.
        var webApp = webApplicationBuilder();
        DefinePath(webApp.Configuration);
        DetectPlugins();
        LoadConfiguration();

        var services = webApp.Services;
        services.AddLunaticPanelServices();
        services.ProcessPlugins();
        EnsurePluginValidatedBlazor();
        BootstrapPluginDescriptor[] activePluginsEntryPoint = Configuration.ActivePlugins.ToArray();
        services.ScanAndAddBusHandlersFor(activePluginsEntryPoint);
        services.AddPluginServices(activePluginsEntryPoint);
        SaveConfiguration();
        return webApp.Build();
    }

    public static Task RunAsync(Func<WebApplication> webApplication)
    {
        var webApp = webApplication();
        var pluginRegistry = webApp.Services.GetRequiredService<IPluginRegistry>();
        foreach (var item in Configuration.ActivePlugins)
        {
            var collection = new ServiceCollection();
            item.EntryPoint!.RegisterServices(collection);

            pluginRegistry.Register(new(item.EntryPointType, item.EntryPoint, item.Entity, collection));
            item.EntryPoint!.Configure(webApp.Configuration);

        }
        webApp.RegisterScannedBusHandlers();

        return webApp.RunAsync();
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
        }
    }
}
