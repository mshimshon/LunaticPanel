using LunaticPanel.Core;
using LunaticPanel.Core.Abstraction.Circuit;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Plugin.Test;

public class Plugin : PluginBase
{
    protected override void RegisterPluginServices(IServiceCollection services, CircuitIdentity circuit)
    {
        services.AddScoped<MyService>();
    }
}
