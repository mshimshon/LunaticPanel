using LunaticPanel.Core;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys;
using Newtonsoft.Json.Linq;

namespace LunaticPanel.Plugin.Test;

public partial class PluginEntry : PluginBase
{
    public override void CheckFeatureDegradation(Func<string, bool> isBusAvailable)
    {
        if (!isBusAvailable(""))
            DisableBusFeature("");
    }

    public override string[] GetMyPackageKeys()
        => typeof(BaseInfo).Assembly.ScanKeyPackageForKeys();


    protected override void RegisterPluginServices(IPluginServiceCollection services, CircuitIdentity circuit)
    {
        JObject json = new JObject();
        services.AddScoped<MyService>();
        services.AddScoped<MenuViewModel>();

    }

}
