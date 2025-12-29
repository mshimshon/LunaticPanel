using LunaticPanel.Core;
using LunaticPanel.Core.Plugin;
using LunaticPanel.Engine.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        // BUILDING THE PLUGIN SERVICES
        foreach (var d in pluginItem.Services.AsReadOnly())
            serviceCollection.Add(d);

        // PLUGIN SERVICE WILL INHERIT VIA PROXY THE CIRCUIT SCOPED PROVIDER SERVICES
        var copiedHostList = new List<ServiceDescriptor>();
        foreach (var item in HostServiceStorage.HostServices)
            copiedHostList
                .CopyScopedServiceAsProxy(item, serviceProvider)
                .CopySingletonServiceAsProxy(item, serviceProvider)
                .CopyTransientAndOpenGeneric(item);

        foreach (var d in copiedHostList)
            if (!serviceCollection.Any(x => x.ServiceType != d.ServiceType))
                serviceCollection.Add(d);

        // Get Root PLugin Provider
        // The RPP is a "singleton" across the app... it role is to register and serve circuit Singleton/Scoped level services AND plugin singleton services across all circuits the same way circuit singleton are shared.
        var rootPluginProvider = pluginRegistry.GetRootProvider(pluginItem.Entry);
        if (rootPluginProvider == default)
        {
            var rootPluginProviderInstance = serviceCollection.BuildServiceProvider();
            var rootPluginScope = rootPluginProviderInstance.CreateScope();
            rootPluginProvider = rootPluginScope.ServiceProvider;
            pluginRegistry.AddRootProvider(pluginItem.Entry, rootPluginProvider);
        }

        // NOW we recreate the third and final layer, the plugin service provider itself proxying to rootPlugin Singletons and Host Level Scoped only
        var serviceCollectionSecond = new ServiceCollection();
        var copiedPluginServicesSecondTime = new List<ServiceDescriptor>();
        foreach (var d in pluginItem.Services.AsReadOnly())
            copiedPluginServicesSecondTime
                .CopySingletonServiceAsProxy(d, rootPluginProvider)
                .CopyScopedService(d)
                .CopyTransientAndOpenGeneric(d);
        foreach (var d in copiedPluginServicesSecondTime)
            serviceCollectionSecond.Add(d);

        var copiedHostListSecondTime = new List<ServiceDescriptor>();
        foreach (var item in HostServiceStorage.HostServices)
            copiedHostListSecondTime
                .CopyScopedServiceAsProxy(item, rootPluginProvider)
                .CopySingletonServiceAsProxy(item, rootPluginProvider)
                .CopyTransientAndOpenGeneric(item);

        foreach (var d in copiedHostListSecondTime)
            if (!serviceCollectionSecond.Any(x => x.ServiceType != d.ServiceType))
                serviceCollectionSecond.Add(d);

        var pluginProvider = serviceCollectionSecond.BuildServiceProvider();
        _pluginScope = pluginProvider.CreateScope();
    }

    public TService GetRequired<TService>() where TService : notnull
        => InternalProvider.GetRequiredService<TService>();








}