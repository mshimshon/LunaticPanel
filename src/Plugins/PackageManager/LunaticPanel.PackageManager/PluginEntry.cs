using LunaticPanel.Core;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Extensions;
using LunaticPanel.PackageManager.Infrastructure;
using LunaticPanel.PackageManager.Keys;
using LunaticPanel.PackageManager.Pages;
using LunaticPanel.PackageManager.Pages.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
public class PluginEntry : PluginBase
{
    public override void CheckFeatureDegradation(Func<string, bool> isBusAvailable)
    {

    }

    public override string[] GetMyPackageKeys()
        => typeof(PackageManagerKeys).Assembly.ScanKeyPackageForKeys();

    protected override void RegisterPluginServices(IServiceCollection services, CircuitIdentity circuit)
    {
        services.AddScoped<IHomeViewModel, HomeViewModel>();
        services.AddInfrasctructureServices();
    }
    protected override void LoadConfiguration(IConfiguration configuration)
    {

    }
}
