namespace LunaticPanel.Engine.Extensions;

public static class DependencyInjectionHelperExt
{

    public static List<ServiceDescriptor> CopySingletonServiceAsProxy(this List<ServiceDescriptor> services, ServiceDescriptor d, IServiceProvider proxyFrom)
    {
        if (!d.ServiceType.IsGenericTypeDefinition && d.Lifetime == ServiceLifetime.Singleton)
            services.Add(ServiceDescriptor.Describe(d.ServiceType, sp => proxyFrom.GetRequiredService(d.ServiceType), d.Lifetime));

        return services;
    }

    public static List<ServiceDescriptor> CopyScopedServiceAsProxy(this List<ServiceDescriptor> services, ServiceDescriptor d, IServiceProvider proxyFrom)
    {
        if (!d.ServiceType.IsGenericTypeDefinition && d.Lifetime == ServiceLifetime.Scoped)
            services.Add(ServiceDescriptor.Describe(d.ServiceType, sp => proxyFrom.GetRequiredService(d.ServiceType), d.Lifetime));
        return services;
    }

    public static List<ServiceDescriptor> CopyScopedService(this List<ServiceDescriptor> services, ServiceDescriptor d)
    {
        if (!d.ServiceType.IsGenericTypeDefinition && d.Lifetime == ServiceLifetime.Scoped)
            if (d.ImplementationType == null && d.ImplementationFactory != null)
                services.Add(ServiceDescriptor.Describe(d.ServiceType, sp => d.ImplementationFactory(sp)!, d.Lifetime));
            else
                services.Add(ServiceDescriptor.Describe(d.ServiceType, d.ImplementationType!, d.Lifetime));
        return services;
    }

    public static List<ServiceDescriptor> CopyTransientAndOpenGeneric(this List<ServiceDescriptor> services, ServiceDescriptor d)
    {
        if (d.ServiceType.IsGenericTypeDefinition || d.Lifetime == ServiceLifetime.Transient)
            if (d.ImplementationType == null && d.ImplementationFactory != null)
                services.Add(ServiceDescriptor.Describe(d.ServiceType, sp => d.ImplementationFactory(sp)!, d.Lifetime));
            else
                services.Add(ServiceDescriptor.Describe(d.ServiceType, d.ImplementationType!, d.Lifetime));
        return services;
    }

}
