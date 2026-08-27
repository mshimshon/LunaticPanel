using LunaticPanel.Core;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys;
using Newtonsoft.Json.Linq;

namespace LunaticPanel.Plugin.Test;

public partial class PluginEntry : PluginBase, IPlugin
{
    public override void CheckFeatureDegradation(Func<string, bool> isBusAvailable)
    {
        if (!isBusAvailable(""))
            DisableBusFeature("");
    }

    public override string[] GetMyPackageKeys()
        => typeof(LPEngineKeys).Assembly.ScanKeyPackageForKeys();


    protected override void RegisterPluginServices(IPluginServiceCollection services, CircuitIdentity circuit)
    {
        JObject json = new JObject();
        services.AddScoped<MyService>();
        services.AddScoped<MenuViewModel>();

    }

}
