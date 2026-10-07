using Microsoft.Extensions.DependencyInjection;
namespace LunaticPanel.Core.Abstraction.DependencyInjection;


public interface IPluginCrossCircuitServiceCollection
{
    IServiceCollection CrossCircuitServices { get; }
    IServiceCollection AddCrossCircuitSingleton(Type serviceType, Type implementationType);
    IServiceCollection AddCrossCircuitSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory);
    IServiceCollection AddCrossCircuitSingleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;
    IServiceCollection AddCrossCircuitSingleton(Type serviceType);
    IServiceCollection AddCrossCircuitSingleton<TService>()
        where TService : class;
    IServiceCollection AddCrossCircuitSingleton(Type serviceType, object implementationInstance);
    IServiceCollection AddCrossCircuitSingleton<TService>(TService implementationInstance)
        where TService : class;
    IServiceCollection AddCrossCircuitSingleton<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class;

    IServiceCollection AddCrossCircuitSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService;
}
