using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Utils.Abstraction.IO;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.IO;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Services;
using LunaticPanel.Engine.Web.Bootstraper.Discovery;
using LunaticPanel.Engine.Web.Bootstraper.Discovery.Services;
using LunaticPanel.Engine.Web.Services;
using LunaticPanel.Engine.Web.Services.Circuit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace LunaticPanel.Engine.Web.Bootstraper.Engine;

public class Bootstraper : IBootstrap, IPluginRegistry, IDisposable
{
    private readonly PluginDiscoveryLocation _discoveryLocation;

    private readonly PluginDiscovery _pluginDiscovery;

    private readonly UpdateDiscoveryLocation _upgradeLocation;
    private readonly UpdateDiscovery _upgradeDiscovery;
    private List<Assembly> _additionalAssemblies = new();
    private List<PluginLifespanController> _activePlugins = new();
    private List<HostRedirectionService> _redirectHostService;
    private bool _disposedValue = false;
    public IPluginDiscoveryLocator DiscoveryLocation => _discoveryLocation;
    public IUpdateDiscoveryLocator UpgradeLocation => _upgradeLocation;
    public IPluginDiscovery PluginDiscovery => _pluginDiscovery;

    public IUpdateDiscovery UpgradeDiscovery => _upgradeDiscovery;
    public IReadOnlyList<Assembly> AdditionalAssemblies => _additionalAssemblies;
    public IReadOnlyList<IPluginLifespanController> ActivePlugins => _activePlugins;
    private IServiceCollection _services;
    private IConfiguration _configuration;

    public IReadOnlyList<HostRedirectionService> RedirectHostService => _redirectHostService;
    private IServiceScope _masterScope = default!;

    public IServiceProvider MasterSp { get; private set; } = default!;

    public IReadOnlyList<BootstrapStatusEntity> KnownPlugins => PluginDiscovery.BootstrapState;

    public IReadOnlyDictionary<string, string> FailedPlugins { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private IPluginStateController _pluginStateController;
    internal Bootstraper(IServiceCollection services, IConfiguration configuration)
    {
        _pluginStateController = new PluginStateController();
        services.AddSingleton<IBootstrap>((sp) => this);
        services.AddSingleton<IPluginRegistry>((sp) => this);
        _discoveryLocation = new PluginDiscoveryLocation();
        _pluginDiscovery = new PluginDiscovery(DiscoveryLocation, _pluginStateController);
        _upgradeLocation = new UpdateDiscoveryLocation();
        _upgradeDiscovery = new UpdateDiscovery(UpgradeLocation, DiscoveryLocation);
        _services = services;
        _configuration = configuration;
        _redirectHostService = RegisterServicesExt
            .AddHostRedirectedServices(new ServiceCollection())
            .Select(p => new HostRedirectionService(p.ServiceType, p.Lifetime)).ToList();
    }
    private PluginLifespanController BindController(IPluginReference pluginReference, WebApplication webApp, IServiceProvider masterSp)
    {
        return new PluginLifespanController(
            pluginReference, DiscoveryLocation, PluginDiscovery, UpgradeLocation, UpgradeDiscovery, webApp,
            _configuration, masterSp, _redirectHostService.Select(p => p with { }).ToArray());

    }
    public async Task SetupAsync(WebApplication webApp, IServiceProvider serviceProvider)
    {
        _masterScope = serviceProvider.CreateScope();
        MasterSp = _masterScope.ServiceProvider;
        try
        {
            await UpgradeDiscovery.ApplyUpgrade();
        }
        catch (System.Exception ex)
        {
            CrazyReportConsole.ReportError(ex.Message);
            Environment.Exit(1);
            throw;
        }
        PluginDiscovery.Commit_All();
        _activePlugins = PluginDiscovery.LoadedPlugins.Select(p => BindController(p, webApp, MasterSp)).ToList();
        FailedPlugins = _pluginDiscovery.BootstrapState.Where(p => p.Failure != default).ToDictionary(p => p.Manifest.Id, p => p.Failure!, StringComparer.OrdinalIgnoreCase);

        // Set Assembly for Blazor
        _additionalAssemblies =
            [.. _activePlugins.Select(p => p.Plugin!.GetType().Assembly!), typeof(RegisterServicesExt).Assembly];

    }

    /*
        The idea is that every time a dependent plugin find a missing key it internally disable their own bus depending
        on that key then, the loop start again with the disabled key no longer in the pool disabling other dependency
        that were dependent on the removed key over and over until no more missing dependencies are found.
    */
    internal async Task SetupGracefulDegradation()
    {
        bool restart = false;
        CrazyReportConsole.ReportInfo("Load Graceful Feature Plugin Degradation");

        do
        {
            restart = false;

            var pluginAndKeys = ActivePlugins.ToDictionary(p => p, p => p.PluginInfo.Keys);
            var globalKeys = pluginAndKeys.SelectMany(p => p.Value);
            foreach (IPluginLifespanController plugin in ActivePlugins)
            {
                var newKeys = plugin.Plugin.CheckDependencyGracefully(p => globalKeys.Any(x => x.SameAs(p)));
                var cnt = newKeys.Count - pluginAndKeys[plugin].Count();
                restart = cnt != 0;
                if (restart)
                {
                    CrazyReportConsole.ReportWarning("{0} has reported degradation of {1} bus features.", plugin.Manifest.Id, Math.Abs(cnt));
                    break;
                }
            }
            await Task.Delay(10);
        } while (restart);
        CrazyReportConsole.ReportSuccess("Finished Plugin Degradation Process");
    }

    internal async Task ProcessPlugins()
    {
        Console.WriteLine($"Configure Active Plugins ({ActivePlugins.Count})");
        foreach (IPluginLifespanController plugin in ActivePlugins)
            await plugin.InitializeAsync();

        await SetupGracefulDegradation();


        await MasterSp.RuntimeStartupBeforePluginsAsync();

        foreach (IPluginLifespanController plugin in ActivePlugins)
            await plugin.SignalBeforeRuntimeAsync();
    }
    public async Task RunAsync(IServiceProvider serviceProvider)
    {

        var circuitRegistry = MasterSp.GetRequiredService<CircuitRegistry>();
        circuitRegistry.SelfCircuitRegistration(Guid.NewGuid(), default);
        await ProcessPlugins();
        await MasterSp.RuntimeStartupAfterPluginsAsync();
        await MasterSp.RuntimeHostStartup();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _masterScope.Dispose();
                _redirectHostService.Clear();
                _additionalAssemblies.Clear();
                _pluginDiscovery.Dispose();
                _activePlugins = new();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public IPluginLifespanController GetById(string id) => ActivePlugins.Single(p => p.Manifest.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    public IPluginLifespanController? GetByIdOrDefault(string id) => ActivePlugins.SingleOrDefault(p => p.Manifest.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
}
