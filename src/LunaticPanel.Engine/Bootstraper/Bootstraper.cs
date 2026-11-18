using Microsoft.Extensions.Configuration;
using System;

namespace LunaticPanel.Engine.Web.Bootstraper;

public class Bootstraper
{
    public static Bootstraper Create() => new();
    public PluginDiscoveryLocation DiscoveryLocation { get; }
    public PluginDiscovery Discovery { get; }
    public Bootstraper()
    {
        DiscoveryLocation = new PluginDiscoveryLocation();
        Discovery = new PluginDiscovery(DiscoveryLocation);
    }
    public async Task Setup(IServiceCollection services, IConfiguration configuration)
    {
        Discovery.Commit_All();
    }
}
