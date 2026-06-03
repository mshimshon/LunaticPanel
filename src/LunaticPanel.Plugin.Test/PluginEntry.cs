using LunaticPanel.Core;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace LunaticPanel.Plugin.Test;

public partial class PluginEntry : PluginBase
{
    public override string[] GetMyPackageKeys()
        => typeof(BaseInfo).Assembly.ScanKeyPackageForKeys();

    protected override void RegisterPluginServices(IServiceCollection services, CircuitIdentity circuit)
    {
        JObject json = new JObject();
        services.AddScoped<MyService>();
        services.AddScoped<MenuViewModel>();
    }
}
