using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Utils.Abstraction.Logging;
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


    private static async Task ConfigureActivePlugin(BootstrapPluginDescriptor plugin,
        WebApplication webApp,
        IServiceProvider masterSp,
        IConfiguration configuration,
        ICrazyReport crazyReport)
    {
        var pluginRegistry = masterSp.GetRequiredService<IPluginRegistry>();

        pluginRegistry.Register(new(plugin.EntryPoint!, plugin.Entity));
        string linuxName = plugin.Entity.Identity.PackageId.Replace('.', '_').ToLower();
        string dynamicWwwRoot = $"/etc/lunaticpanel/plugins/{linuxName}/wwwroot";
        if (!Directory.Exists(dynamicWwwRoot) && OperatingSystem.IsLinux())
            Directory.CreateDirectory(dynamicWwwRoot, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                UnixFileMode.GroupRead | UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherExecute);

        crazyReport.ReportInfo("Loading Plugin: {0}", plugin.Entity.Identity.DisplayName);
        crazyReport.ReportInfo("Location: {0}", plugin.PluginDir);
        crazyReport.ReportInfo("PackageId: {0}", plugin.Entity.Identity.PackageId);
        var dynamicContentOption = new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(dynamicWwwRoot),
            RequestPath = $"/_plugins/dynamic/{plugin.Entity.Identity.PackageId}"
        };
        crazyReport.ReportInfo("Location (Dynamic Assets): {0}", dynamicWwwRoot);
        webApp.UseStaticFiles(dynamicContentOption);

        var wwwroot = Path.Combine(plugin.PluginDir, "wwwroot");
        if (Directory.Exists(wwwroot))
        {
            crazyReport.ReportInfo("Location (Static Assets): {0}", wwwroot);
            var options = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(wwwroot),
                RequestPath = $"/_plugins/static/{plugin.Entity.Identity.PackageId}"
            };
            if (webApp != default)
                webApp.UseStaticFiles(options);
        }
        else
            crazyReport.ReportWarning("Location (Static Assets): '{0}' Doesn't Exist", wwwroot);


        var redirectServiceToHost = RegisterServicesExt
            .AddHostRedirectedServices(new ServiceCollection())
            .Select(p => new HostRedirectionService(p.ServiceType, p.Lifetime))
            .ToArray();
        plugin.EntryPoint!.AddHostRedirectedServices(redirectServiceToHost);
        crazyReport.ReportSuccess("Plugin {0} has been loaded.", plugin.Entity.Identity.DisplayName);
    }
    private static async Task InitializeActivePlugin(BootstrapPluginDescriptor plugin,
        WebApplication webApp,
        IServiceProvider masterSp,
        IConfiguration configuration,
        ICrazyReport crazyReport)
    {
        crazyReport.ReportInfo("Loading Plugin: {0}", plugin.Entity.Identity.DisplayName);
        crazyReport.ReportInfo("Location: {0}", plugin.PluginDir);
        crazyReport.ReportInfo("PackageId: {0}", plugin.Entity.Identity.PackageId);
        await plugin.EntryPoint!.BeforeRuntimeStartAsync(masterSp);
        crazyReport.ReportSuccess("Plugin {0} has been initialized.", plugin.Entity.Identity.DisplayName);
    }
    public static async Task BootstrapRunAsync(WebApplication webApp, IServiceProvider serviceProvider, IConfiguration configuration)
    {

        var masterSp = serviceProvider.CreateScope().ServiceProvider;
        var crazyReport = masterSp.GetRequiredService<ICrazyReport>();
        crazyReport.SetClass(nameof(Bootstrap));
        crazyReport.SetModule("Plugin");

        var circuitRegistry = masterSp.GetRequiredService<CircuitRegistry>();
        var pluginRegistry = masterSp.GetRequiredService<IPluginRegistry>();

        foreach (BootstrapPluginDescriptor plugin in Configuration.ActivePlugins)
            await ConfigureActivePlugin(plugin, webApp, masterSp, configuration, crazyReport);

        circuitRegistry.SelfCircuitRegistration(Guid.NewGuid(), default);
        await masterSp.RuntimeStartupBeforePluginsAsync();

        foreach (BootstrapPluginDescriptor plugin in Configuration.ActivePlugins)
            await InitializeActivePlugin(plugin, webApp, masterSp, configuration, crazyReport);


        await masterSp.RuntimeStartupAfterPluginsAsync();
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
