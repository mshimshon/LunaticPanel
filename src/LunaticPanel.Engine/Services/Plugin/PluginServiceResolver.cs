using LunaticPanel.Core;
using LunaticPanel.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;
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
        foreach (var d in pluginItem.Services)
            serviceCollection.Add(d);

        var hostCircuitProvider = CopyHostServices(serviceCollection, HostServiceStorage.HostServices, serviceProvider);

        foreach (var d in hostCircuitProvider)
            serviceCollection.Add(d);

        var pluginProvider = serviceCollection.BuildServiceProvider();
        _pluginScope = pluginProvider.CreateScope();
    }

    public TService GetRequired<TService>() where TService : notnull
        => InternalProvider.GetRequiredService<TService>();

    public static List<ServiceDescriptor> CopyHostServices(
        IServiceCollection pluginServices,
        IReadOnlyCollection<ServiceDescriptor> hostServices,
        IServiceProvider hostScope)
    {
        var result = new List<ServiceDescriptor>();

        foreach (var d in hostServices)
        {
            if (pluginServices.Any(x => x.ServiceType == d.ServiceType))
                continue;

            bool isOpenGeneric = d.ServiceType.IsGenericTypeDefinition;
            bool isTransient = d.Lifetime == ServiceLifetime.Transient;

            if (isOpenGeneric || isTransient)
            {
                // clone
                result.Add(ServiceDescriptor.Describe(
                    d.ServiceType,
                    d.ImplementationType!,
                    d.Lifetime));
            }
            else
            {
                // proxy
                result.Add(ServiceDescriptor.Describe(
                    d.ServiceType,
                    sp => hostScope.GetRequiredService(d.ServiceType),
                    d.Lifetime));
            }
        }


        return result;
    }






}