using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Logging;
using LunaticPanel.Engine.Plugin;

using LunaticPanel.Engine.Web.Bootstraper.Discovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;

namespace LunaticPanel.Engine.Web.Bootstraper.Engine;

public class PluginLifespanController : IPluginLifespanController
{
    private readonly IPluginDiscoveryLocator _pluginDiscoveryLocation;
    private readonly IPluginDiscovery _pluginDiscovery;
    private readonly IUpdateDiscoveryLocator _updateDiscoveryLocation;
    private readonly IUpdateDiscovery _updateDiscovery;
    private readonly WebApplication _webApplication;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _masterSp;
    private readonly HostRedirectionService[] _hostPluginServices;
    private bool _disposedValue;

    public PluginManifestEntity Manifest => PluginReference.Manifest;
    public IPluginReference PluginReference { get; }
    public IPluginInfo PluginInfo => PluginReference.Entry;
    public IPlugin Plugin => PluginReference.Entry;
    public string PluginDir { get; }
    public string PluginVarDir { get; }
    public string AssetStaticDir { get; }
    public string AssetDynamicDir { get; }
    private UnixFileMode _publicReadPermission = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                    UnixFileMode.GroupRead | UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherExecute;
    public PluginLifespanController(IPluginReference pluginReference,
        IPluginDiscoveryLocator pluginDiscoveryLocation,
        IPluginDiscovery pluginDiscovery,
        IUpdateDiscoveryLocator updateDiscoveryLocation,
        IUpdateDiscovery updateDiscovery,
        WebApplication webApplication,
        IConfiguration configuration,
        IServiceProvider masterSp,
        HostRedirectionService[] hostPluginServices
        )
    {
        PluginReference = pluginReference;
        _pluginDiscoveryLocation = pluginDiscoveryLocation;
        _pluginDiscovery = pluginDiscovery;
        _updateDiscoveryLocation = updateDiscoveryLocation;
        _updateDiscovery = updateDiscovery;
        _webApplication = webApplication;
        _configuration = configuration;
        _masterSp = masterSp;
        _hostPluginServices = hostPluginServices;

        PluginDir = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, Manifest.FolderId);
        AssetStaticDir = Pathf.CombineEnsureCreated(_publicReadPermission, PluginDir, "wwwroot");
        PluginVarDir = Pathf.CombineEnsureCreated(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel", "plugins", Manifest.FolderId);
        AssetDynamicDir = Pathf.CombineEnsureCreated(_publicReadPermission, PluginVarDir, "wwwroot");

        Configure();
    }
    private void Configure()
    {

    }

    public void ConfigureStaticAssets()
    {
        var dynamicContentOption = new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(AssetDynamicDir),
            RequestPath = $"/_plugins/dynamic/{Manifest.Id}"
        };
        CrazyReportConsole.ReportInfo("Location (Dynamic Assets): {0}", AssetDynamicDir);

        _webApplication.UseStaticFiles(dynamicContentOption);

        CrazyReportConsole.ReportInfo("Location (Static Assets): {0}", AssetStaticDir);
        var options = new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(AssetStaticDir),
            RequestPath = $"/_plugins/static/{Manifest.Id}"
        };
        _webApplication.UseStaticFiles(options);
    }
    public async Task InitializeAsync()
    {
        CrazyReportConsole.ReportInfo("Initializing...");
        PrintPluginInfo();
        ConfigureStaticAssets(); // Define Static Assets


        CrazyReportConsole.Report("Apply to Plugin");
        Plugin.AddHostRedirectedServices(_hostPluginServices);
    }

    public void PrintPluginInfo()
    {
        string border = new string('-', 50);
        CrazyReportConsole.ReportInfo($"+{border}+");
        CrazyReportConsole.ReportInfo($"| {"Plugin Information",-48} |");
        CrazyReportConsole.ReportInfo($"+{border}+");
        CrazyReportConsole.ReportInfo($"| {"Name:",-12} {Manifest.Title,-35} |");
        CrazyReportConsole.ReportInfo($"| {"Id:",-12} {Manifest.Id,-35} |");
        CrazyReportConsole.ReportInfo($"| {"Version:",-12} {Manifest.Version,-35} |");
        CrazyReportConsole.ReportInfo($"| {"Panel SDK:",-12} {Manifest.PanelVersion,-35} |");
        CrazyReportConsole.ReportInfo($"| {"Location:",-12} {PluginDir,-35} |");
        CrazyReportConsole.ReportInfo($"+{border}+");
    }

    public async Task SignalBeforeRuntimeAsync()
    {
        CrazyReportConsole.ReportInfo("Triggering Before Runtime...");
        PrintPluginInfo();
        await Plugin.BeforeRuntimeStartAsync(_masterSp);
        CrazyReportConsole.ReportSuccess("Plugin {0} has been initialized.", Manifest.Title);
    }

}
