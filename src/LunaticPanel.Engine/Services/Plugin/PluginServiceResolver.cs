using LunaticPanel.Core;
using LunaticPanel.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Services.Plugin;

internal class PluginServiceResolver<TPlugin> : IPluginService<TPlugin>
    where TPlugin : IPlugin
{

    private readonly IServiceScope _pluginScope;
    private IServiceProvider InternalProvider { get => _pluginScope.ServiceProvider; }
    public PluginServiceResolver(IServiceProvider serviceProvider, PluginRegistry pluginRegistry)
    {

        var pluginItem = pluginRegistry.GetByEntryType(typeof(TPlugin));
        var serviceCollection = new ServiceCollection();
        pluginItem.Entry.RegisterServices(serviceCollection);
        foreach (var d in HostServiceStorage.HostServices)
        {
            if (serviceCollection.Any(x => x.ServiceType == d.ServiceType))
                continue;
            if (d.ServiceType.IsGenericTypeDefinition)
                continue;
            if (d.Lifetime == ServiceLifetime.Singleton)
            {
                var instance = serviceProvider.GetRequiredService(d.ServiceType);
                serviceCollection.AddSingleton(d.ServiceType, instance);
            }
            else if (d.Lifetime == ServiceLifetime.Scoped)
            {
                serviceCollection.AddScoped(d.ServiceType, _ => serviceProvider.GetRequiredService(d.ServiceType));
            }
        }
        var pluginProvider = serviceCollection.BuildServiceProvider();
        _pluginScope = pluginProvider.CreateScope();
    }

    public TService GetRequired<TService>() where TService : notnull
        => InternalProvider.GetRequiredService<TService>();
}
