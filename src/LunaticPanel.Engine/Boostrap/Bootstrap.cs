using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Plugin;
using LunaticPanel.Engine.Infrastructure;
using LunaticPanel.Engine.Infrastructure.Circuit;
using LunaticPanel.Engine.Services;
using LunaticPanel.Engine.Services.Messaging;
using LunaticPanel.Engine.Services.Messaging.EngineBus;
using LunaticPanel.Engine.Services.Plugin;
using MudBlazor;
using MudBlazor.Services;
using StatePulse.Net;
using SwizzleV;
using System.Reflection;
using System.Text.Json;
using static LunaticPanel.Engine.Boostrap.BootstrapPlugins;
using static LunaticPanel.Engine.Boostrap.BootstrapPluginsBlazorValidator;
namespace LunaticPanel.Engine.Boostrap;

public static class Bootstrap
{
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
        DefinePath();
        DetectPlugins();
        LoadConfiguration();
        var webApp = webApplicationBuilder();
        var services = webApp.Services;
        services.AddServices().SealServiceCollection();
        services.ScanBusHandlers();
        services.ProcessPlugins();
        EnsurePluginValidatedBlazor();
        BootstrapPluginDescriptor[] activePluginsEntryPoint = Configuration.ActivePlugins.ToArray();
        services.ScanAndAddBusHandlersFor(activePluginsEntryPoint);
        services.AddPluginServiceResolverFor(activePluginsEntryPoint);

        Routes.AdditionalAssemblies = activePluginsEntryPoint.Select(p => p.EntryPointType.Assembly).ToList();
        SaveConfiguration();
        return webApp.Build();
    }

    public static Task RunAsync(Func<WebApplication> webApplication)
    {
        var webApp = webApplication();
        var pluginRegistry = webApp.Services.GetRequiredService<PluginRegistry>();
        foreach (var item in Configuration.ActivePlugins)
        {
            var collection = new ServiceCollection();
            item.EntryPoint!.RegisterServices(collection);

            pluginRegistry.Register(new(item.EntryPointType, item.EntryPoint, item.Entity, collection));
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
    private static void DefinePath()
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        PluginDirectory = Path.Combine(appDataDir, "LunaticPanel", "Plugins");
        ConfigDirectory = Path.Combine(appDataDir, "LunaticPanel");

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

    public static void SealServiceCollection(this IServiceCollection services)
    {
        HostServiceStorage.HostServices = services.ToList().AsReadOnly();

    }
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddEngineInfrastructure();
        services.AddScoped<CircuitRegistry>();
        services.AddScoped<ICircuitControl, CircuitRegistry>();
        services.AddSingleton<EngineBusRegistry>();
        services.AddScoped<IEngineBus, EngineBus>();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        services.AddStatePulseServices(o =>
        {
            o.ScanAssemblies = [
                typeof(RegisterServicesExt),
                typeof(Application.RegisterServicesExt),
                typeof(Infrastructure.RegisterServicesExt)
                ];
        });

        services.AddSingleton<PluginRegistry>();
        services.AddSwizzleV();
        services.ScanBusHandlers();
        return services;
    }


    private static void ScanAndAddBusHandlersFor(this IServiceCollection services, params BootstrapPluginDescriptor[] plugins)
    {
        foreach (var item in plugins)
            services.ScanBusHandlers(item.EntryPoint);
    }
    private static void AddPluginServiceResolverFor(this IServiceCollection services, params BootstrapPluginDescriptor[] plugins)
    {
        foreach (var item in plugins)
        {
            var pluginType = item.EntryPointType;
            var serviceType = typeof(IPluginService<>).MakeGenericType(pluginType);
            var resolverType = typeof(PluginServiceResolver<>).MakeGenericType(pluginType);
            services.AddScoped(serviceType, resolverType);
        }
    }




}
