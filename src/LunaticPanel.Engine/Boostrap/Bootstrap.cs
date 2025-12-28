using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Plugin;
using LunaticPanel.Engine.Infrastructure;
using LunaticPanel.Engine.Infrastructure.Circuit;
using LunaticPanel.Engine.Services;
using LunaticPanel.Engine.Services.Messaging;
using LunaticPanel.Engine.Services.Messaging.EngineBus;
using LunaticPanel.Engine.Services.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using StatePulse.Net;
using SwizzleV;
using System.Reflection;
using System.Text.Json;
using static LunaticPanel.Engine.Boostrap.BootstrapPlugins;
namespace LunaticPanel.Engine.Boostrap;

public static class Bootstrap
{
    public static string PluginDirectory { get; private set; } = default!;
    public static string ConfigDirectory { get; private set; } = default!;
    internal static BootstrapConfiguration Configuration { get; private set; } = new();
    public static UnixFileMode DefaultDirectoryPermissions { get; } =
        UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
        UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
        UnixFileMode.OtherRead | UnixFileMode.OtherExecute;

    public static WebApplication Load(Func<WebApplicationBuilder> webApplicationBuilder)
    {
        DefinePath();
        DetectPlugins();
        LoadConfiguration();
        var webApp = webApplicationBuilder();
        var services = webApp.Services;
        services.AddServices().SealServiceCollection();
        services.ScanBusHandlers();
        services.ProcessPlugins();
        var activePluginsEntryPoint = Configuration.ActivePlugins.Select(p => p.EntryPoint!).ToArray();
        services.ScanAndAddBusHandlersFor(activePluginsEntryPoint);
        services.AddPluginServiceResolverFor(activePluginsEntryPoint);
        SaveConfiguration();
        return webApp.Build();
    }

    public static Task RunAsync(Func<WebApplication> webApplication)
    {
        var webApp = webApplication();
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


    private static void ScanAndAddBusHandlersFor(this IServiceCollection services, params IPlugin[] plugins)
    {
        foreach (var item in plugins)
            services.ScanBusHandlers(item);
    }
    private static void AddPluginServiceResolverFor(this IServiceCollection services, params IPlugin[] plugins)
    {
        foreach (var item in plugins)
        {
            var pluginType = item.GetType();
            var serviceType = typeof(IPluginService<>).MakeGenericType(pluginType);
            var resolverType = typeof(PluginServiceResolver<>).MakeGenericType(pluginType);
            services.AddScoped(serviceType, resolverType);
        }
    }

    private static IApplicationBuilder AddAdditionalAssemblies(this IApplicationBuilder builder, params Assembly[] assemblies)
    {
        Routes.AdditionalAssemblies.AddRange(assemblies);
        return builder;
    }

}
