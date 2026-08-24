using LunaticPanel.Core;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Components.ViewModels;
using LunaticPanel.PackageManager.Infrastructure;
using LunaticPanel.PackageManager.Keys;
using LunaticPanel.PackageManager.Pages;
using LunaticPanel.PackageManager.Pages.ViewModels;
using Microsoft.Extensions.Configuration;
using StatePulse.Net;

namespace LunaticPanel.PackageManager;
/*
 *  - Be able to local packages from local folder /etc/LunaticPanel/plugins/LunaticPanel_PackageManager/.local 
 *  - Be able to load all custom .local nuget packages in memory to query with local active.
 *  - You select or define nuget sources
    <dependencies>
      <!-- Change whatever the developer targeted to your custom TFM -->
      <group targetFramework="netpanel1.0">
        <dependency id="YourPanel.SDK" version="1.0.0" />
      </group>
    </dependencies>
 *  
 */
public class PluginEntry : PluginBase, IPlugin
{
    public override void CheckFeatureDegradation(Func<string, bool> isBusAvailable)
    {

    }

    public override string[] GetMyPackageKeys()
        => typeof(LPPackageManagerKeys).Assembly.ScanKeyPackageForKeys();

    protected override void RegisterPluginServices(IPluginServiceCollection services, CircuitIdentity circuit)
    {
        services.AddTransient<IPackageInstalledCardViewModel, PackageInstalledCardViewModel>();
        services.AddTransient<ISourceManagerViewModel, SourceManagerViewModel>();
        services.AddScoped<IPackageInstalledViewModel, PackageInstalledViewModel>();
        services.AddScoped<IHomeViewModel, HomeViewModel>();
        services.AddScoped<IPackageSearchViewModel, PackageSearchViewModel>();
        services.AddScoped<IPackageSearchCardViewModel, PackageSearchCardViewModel>();
        services.AddScoped<ISourceViewModel, SourceViewModel>();
        services.AddScoped<ISourceManagerCardViewModel, SourceManagerCardViewModel>();
        services.AddInfrasctructureServices();
        services.EnableStatePulse();

    }
    protected override void LoadConfiguration(IConfiguration configuration)
    {

    }
    protected override async Task BeforeRuntimeStart(IPluginContextService pluginContext)
    {
        if (!pluginContext.IsMasterCircuit) return;
        var dispatch = pluginContext.GetRequired<IDispatcher>();
        await dispatch.Prepare<LoadSourcesAction>().Await().DispatchAsync();
    }

}
