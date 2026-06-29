using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Abstraction.DependencyInjection;

public interface IPluginServiceCollection : IPluginCrossCircuitServiceCollection
{
    IServiceCollection Services { get; }
    IServiceCollection AddTransient(Type serviceType, Type implementationType);
    IServiceCollection AddTransient(Type serviceType, Func<IServiceProvider, object> implementationFactory);
    IServiceCollection AddTransient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;
    IServiceCollection AddTransient(Type serviceType);
    IServiceCollection AddTransient<TService>()
        where TService : class;
    IServiceCollection AddTransient<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class;
    IServiceCollection AddTransient<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService;

    IServiceCollection AddScoped(Type serviceType, Type implementationType);
    IServiceCollection AddScoped(Type serviceType, Func<IServiceProvider, object> implementationFactory);
    IServiceCollection AddScoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;
    IServiceCollection AddScoped(Type serviceType);
    IServiceCollection AddScoped<TService>()
        where TService : class;
    IServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class;
    IServiceCollection AddScoped<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService;

    IServiceCollection AddSingleton(Type serviceType, Type implementationType);
    IServiceCollection AddSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory);
    IServiceCollection AddSingleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;
    IServiceCollection AddSingleton(Type serviceType);
    IServiceCollection AddSingleton<TService>()
        where TService : class;
    IServiceCollection AddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class;
    IServiceCollection AddSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService;
    IServiceCollection AddSingleton(Type serviceType, object implementationInstance);
    IServiceCollection AddSingleton<TService>(TService implementationInstance)
        where TService : class;



}
