using LunaticPanel.Core.Abstraction.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.DependencyInjection;

internal class PluginServiceCollection : IPluginServiceCollection, IPluginCrossCircuitServiceCollection
{
    public ServiceCollection Collection { get; init; } = new();
    public ServiceCollection CrossCircuitRedirected { get; init; } = new();

    public IServiceCollection Services => Collection;

    public IServiceCollection CrossCircuitServices => CrossCircuitRedirected;

    public ServiceProvider Build() => Collection.BuildServiceProvider();

    public IServiceCollection AddTransient(Type serviceType, Type implementationType)
        => Collection.AddTransient(serviceType, implementationType);

    public IServiceCollection AddTransient(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        => Collection.AddTransient(serviceType, implementationFactory);

    public IServiceCollection AddTransient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
        => Collection.AddTransient<TService, TImplementation>();

    public IServiceCollection AddTransient(Type serviceType)
        => Collection.AddTransient(serviceType);

    public IServiceCollection AddTransient<TService>()
        where TService : class
        => Collection.AddTransient<TService>();

    public IServiceCollection AddTransient<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
        => Collection.AddTransient(implementationFactory);

    public IServiceCollection AddTransient<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
        => Collection.AddTransient<TService, TImplementation>(implementationFactory);

    public IServiceCollection AddScoped(Type serviceType, Type implementationType)
        => Collection.AddScoped(serviceType, implementationType);

    public IServiceCollection AddScoped(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        => Collection.AddScoped(serviceType, implementationFactory);

    public IServiceCollection AddScoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
        => Collection.AddScoped<TService, TImplementation>();

    public IServiceCollection AddScoped(Type serviceType)
        => Collection.AddScoped(serviceType);

    public IServiceCollection AddScoped<TService>()
        where TService : class
        => Collection.AddScoped<TService>();

    public IServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
        => Collection.AddScoped(implementationFactory);

    public IServiceCollection AddScoped<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
        => Collection.AddScoped<TService, TImplementation>(implementationFactory);

    public IServiceCollection AddSingleton(Type serviceType, Type implementationType)
        => Collection.AddSingleton(serviceType, implementationType);

    public IServiceCollection AddSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        => Collection.AddSingleton(serviceType, implementationFactory);

    public IServiceCollection AddSingleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
        => Collection.AddSingleton<TService, TImplementation>();

    public IServiceCollection AddSingleton(Type serviceType)
        => Collection.AddSingleton(serviceType);

    public IServiceCollection AddSingleton<TService>()
        where TService : class
        => Collection.AddSingleton<TService>();

    public IServiceCollection AddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
        => Collection.AddSingleton(implementationFactory);

    public IServiceCollection AddSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
        => Collection.AddSingleton<TService, TImplementation>(implementationFactory);

    public IServiceCollection AddSingleton(Type serviceType, object implementationInstance)
        => Collection.AddSingleton(serviceType, implementationInstance);

    public IServiceCollection AddSingleton<TService>(TService implementationInstance)
        where TService : class
        => Collection.AddSingleton(typeof(TService), implementationInstance);



    public IServiceCollection AddCrossCircuitSingleton(Type serviceType, Type implementationType)
    => CrossCircuitRedirected.AddSingleton(serviceType, implementationType);

    public IServiceCollection AddCrossCircuitSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        => CrossCircuitRedirected.AddSingleton(serviceType, implementationFactory);

    public IServiceCollection AddCrossCircuitSingleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
        => CrossCircuitRedirected.AddSingleton<TService, TImplementation>();

    public IServiceCollection AddCrossCircuitSingleton(Type serviceType)
        => CrossCircuitRedirected.AddSingleton(serviceType);

    public IServiceCollection AddCrossCircuitSingleton<TService>()
        where TService : class
        => CrossCircuitRedirected.AddSingleton<TService>();
    public IServiceCollection AddCrossCircuitSingleton<TService>(Func<IServiceProvider, TService> implementationFactory)
    where TService : class
        => CrossCircuitRedirected.AddSingleton(implementationFactory);

    public IServiceCollection AddCrossCircuitSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
        => CrossCircuitRedirected.AddSingleton<TService, TImplementation>(implementationFactory);
    public IServiceCollection AddCrossCircuitSingleton(Type serviceType, object implementationInstance)
        => CrossCircuitRedirected.AddSingleton(serviceType, implementationInstance);

    public IServiceCollection AddCrossCircuitSingleton<TService>(TService implementationInstance)
        where TService : class
        => CrossCircuitRedirected.AddSingleton(typeof(TService), implementationInstance);
}

