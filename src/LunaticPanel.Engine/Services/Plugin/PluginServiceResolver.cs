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

        var hostCircuitProvider = CopyHostServicesIntoList(serviceCollection, HostServiceStorage.HostServices, serviceProvider, HostServiceStorage.SharedServiceTypes.ToArray());

        foreach (var d in hostCircuitProvider)
            serviceCollection.Add(d);

        var pluginProvider = serviceCollection.BuildServiceProvider();
        _pluginScope = pluginProvider.CreateScope();
    }

    public TService GetRequired<TService>() where TService : notnull
        => InternalProvider.GetRequiredService<TService>();

    public static List<ServiceDescriptor> CopyHostServicesIntoList(
        IServiceCollection pluginServices,
        IReadOnlyCollection<ServiceDescriptor> hostServices,
        IServiceProvider hostCircuitProvider,
        Type[] allowedSharedServices)
    {
        var result = new List<ServiceDescriptor>();

        foreach (var d in hostServices)
        {
            // already registered in plugin
            if (pluginServices.Any(x => x.ServiceType == d.ServiceType))
                continue;

            // not allowed to be shared
            if (!allowedSharedServices.Contains(d.ServiceType))
                continue;

            if (d.ImplementationInstance != null)
            {
                result.Add(new ServiceDescriptor(
                    d.ServiceType,
                    d.ImplementationInstance
                ));
                continue;
            }

            if (d.ImplementationFactory != null)
            {
                result.Add(new ServiceDescriptor(
                    d.ServiceType,
                    _ => d.ImplementationFactory(hostCircuitProvider),
                    d.Lifetime
                ));
                continue;
            }

            if (d.ImplementationType != null)
            {
                if (d.ImplementationType.IsGenericTypeDefinition)
                {
                    result.Add(new ServiceDescriptor(
                        d.ServiceType,
                        d.ImplementationType,
                        d.Lifetime
                    ));
                }
                else
                {
                    result.Add(new ServiceDescriptor(
                        d.ServiceType,
                        _ => hostCircuitProvider.GetRequiredService(d.ServiceType),
                        d.Lifetime
                    ));
                }
            }
        }

        return result;
    }




}