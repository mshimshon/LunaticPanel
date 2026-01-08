using LunaticPanel.Core;
using LunaticPanel.Core.Abstraction.Circuit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace LunaticPanel.Plugin.Test;

public class PluginEntry : PluginBase
{
    protected override void RegisterPluginServices(IServiceCollection services, CircuitIdentity circuit)
    {
        JObject json = new JObject();
        services.AddScoped<MyService>();
        services.AddScoped<MenuViewModel>();
    }
}
