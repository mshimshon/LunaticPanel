using System.Reflection;

namespace LunaticPanel.Engine.Services.Plugin.DependencyController;

public class PluginDependencyInjectionController
{
    private static Dictionary<Type, List<PluginRootInstance>> _rootPluginSingletons = new();
    private static Dictionary<Type, List<ServiceDescriptor>> _rootPluginDescriptor = new();
    private static IReadOnlyDictionary<Type, IReadOnlyList<ServiceDescriptor>> _rootPluginDescriptorCompiled = default!;

    private static Dictionary<Type, List<ServiceDescriptor>> _pluginDescriptor = new();
    private static IReadOnlyDictionary<Type, IReadOnlyList<ServiceDescriptor>> _pluginDescriptorCompiled = default!;

    private static readonly Object _lockRoot = new();
    private static readonly Object _lockPlugin = new();
    private readonly IServiceProvider _hostCircuitProvider;
    private readonly Dictionary<Type, List<PluginScopedInstance>> _pluginScopedInstances = new();

    public PluginDependencyInjectionController(IServiceProvider hostCircuitProvider)
    {
        _hostCircuitProvider = hostCircuitProvider;
    }
    public void Configure(Type pluginType)
    {
        if (!_pluginScopedInstances.ContainsKey(pluginType))
            _pluginScopedInstances[pluginType] = new();
    }
    public static IReadOnlyDictionary<Type, IReadOnlyList<ServiceDescriptor>> GetRootPluginDescriptors()
    {
        if (_rootPluginDescriptorCompiled != default) return _rootPluginDescriptorCompiled;
        lock (_lockRoot)
        {
            _rootPluginDescriptorCompiled = _rootPluginDescriptor.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<ServiceDescriptor>)kvp.Value.AsReadOnly()
            );
            return _rootPluginDescriptorCompiled;

        }

    }

    public static IReadOnlyDictionary<Type, IReadOnlyList<ServiceDescriptor>> GetPluginDescriptors()
    {
        if (_pluginDescriptorCompiled != default) return _pluginDescriptorCompiled;
        lock (_lockRoot)
        {
            _pluginDescriptorCompiled = _pluginDescriptor.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<ServiceDescriptor>)kvp.Value.AsReadOnly()
            );
            return _pluginDescriptorCompiled;

        }

    }
    private static void DefinePlugin(Type pluginType)
    {
        lock (_lockRoot)
        {
            if (!_rootPluginSingletons.ContainsKey(pluginType))
                _rootPluginSingletons[pluginType] = new();
            if (!_rootPluginDescriptor.ContainsKey(pluginType))
                _rootPluginDescriptor[pluginType] = new();
            lock (_lockPlugin)
            {
                if (!_pluginDescriptor.ContainsKey(pluginType))
                    _pluginDescriptor[pluginType] = new();
            }
        }
    }
    public static void AddPluginServices(Type pluginType, List<ServiceDescriptor> serviceDescriptors)
    {
        DefinePlugin(pluginType);
        AddSingletonsOnly(pluginType, serviceDescriptors);
        AddNonSingletonsOnly(pluginType, serviceDescriptors);
    }
    private static void AddSingletonsOnly(Type pluginType, List<ServiceDescriptor> serviceDescriptors)
    {
        lock (_lockRoot)
        {
            var singletons = serviceDescriptors.Where(p => p.Lifetime == ServiceLifetime.Singleton);
            if (!_rootPluginDescriptor.ContainsKey(pluginType)) _rootPluginDescriptor[pluginType] = new();
            _rootPluginDescriptor[pluginType].AddRange(singletons.ToList());
        }
    }

    private static void AddNonSingletonsOnly(Type pluginType, List<ServiceDescriptor> serviceDescriptors)
    {
        lock (_lockPlugin)
        {
            var nonSingletons = serviceDescriptors.Where(p => p.Lifetime != ServiceLifetime.Singleton);
            if (!_pluginDescriptor.ContainsKey(pluginType)) _pluginDescriptor[pluginType] = new();
            _pluginDescriptor[pluginType].AddRange(nonSingletons.ToList());
        }
    }
    public object GetRequiredService(Type pluginType, Type serviceType, int layerUp)
    {

        if (layerUp == 0)
        {
            var pluginList = GetPluginDescriptors()[pluginType];
            if (pluginList != default && pluginList.Any(p => p.ServiceType == serviceType))
                return GetPluginScopedOrTransient(pluginType, serviceType);
            layerUp++;
        }

        if (layerUp == 1)
        {
            var rootList = GetRootPluginDescriptors()[pluginType];
            if (rootList != default && rootList.Any(p => p.ServiceType == serviceType))
                return GetRootPluginSingleton(pluginType, serviceType);
        }

        return GetHostService(serviceType);
    }
    public object GetHostService(Type serviceType) => _hostCircuitProvider.GetRequiredService(serviceType);
    public object GetRootPluginSingleton(Type pluginType, Type serviceType)
    {
        lock (_lockRoot)
        {

            if (!_rootPluginSingletons.ContainsKey(pluginType))
                throw new ArgumentNullException($"{pluginType.FullName} does not own a list of singleton services... this is fatal and most likely internal error.");
            var pluginRootList = _rootPluginSingletons[pluginType];
            var instance = pluginRootList.SingleOrDefault(p => p.ServiceType == serviceType);
            if (instance != default)
                return instance.Instance;
            var _rootPluginDescriptors = _rootPluginDescriptor[pluginType];
            var d = _rootPluginDescriptors.SingleOrDefault(p => p.ServiceType == serviceType);
            if (_rootPluginDescriptors == default)
                return GetHostService(serviceType);
            return CreateInstanceOfService(pluginType, serviceType, 1); // Layer : Host = 2, Root = 1, Plugin = 0
        }
    }

    public object GetPluginScopedOrTransient(Type pluginType, Type serviceType)
    {
        lock (_lockPlugin)
        {


            if (!_pluginScopedInstances.ContainsKey(pluginType))
                throw new ArgumentNullException($"{pluginType.FullName} does not own a list of instance services... this is fatal and most likely internal error.");
            var pluginDescriptors = _pluginDescriptor[pluginType];
            var d = pluginDescriptors.SingleOrDefault(p => p.ServiceType == serviceType);
            if (d == default)
                throw new ArgumentNullException($"{pluginType.FullName} does not own a list of services... this is fatal and most likely internal error.");

            if (d.Lifetime == ServiceLifetime.Scoped)
            {
                var pluginInstances = _pluginScopedInstances[pluginType];
                var instanceScoped = pluginInstances.SingleOrDefault(p => p.ServiceType == serviceType);
                if (instanceScoped == default)
                    return CreateInstanceOfServiceForPlugin(pluginType, serviceType);
                return instanceScoped.Instance;
            }
            return CreateInstanceOfServiceForPlugin(pluginType, serviceType);
        }
    }

    public object CreateInstanceOfServiceForRoot(Type pluginType, Type serviceType)
    {
        lock (_lockRoot)
        {
            var d = _rootPluginDescriptor[pluginType].Single(p => p.ServiceType == serviceType);
            Type implementationType =
                d.ImplementationType ??
                throw new InvalidOperationException($"Service {serviceType} has no implementation type.");

            var ctor = GetConstructorParameterTypesForMissingService(d);
            var args = ctor.Select(p => GetRequiredService(pluginType, p, 0)).ToArray();
            var result = Activator.CreateInstance(implementationType, args)!;
            var plugSingleton = _rootPluginSingletons[pluginType];
            plugSingleton.Add(new(serviceType, result));
            return result;
        }
    }
    public object CreateInstanceOfServiceForPlugin(Type pluginType, Type serviceType)
    {
        lock (_lockPlugin)
        {
            var pluginDescriptors = _pluginDescriptor[pluginType];
            var d = pluginDescriptors.Single(p => p.ServiceType == serviceType);
            if (d.Lifetime == ServiceLifetime.Scoped)
            {
                Type implementationType =
                    d.ImplementationType ??
                    throw new InvalidOperationException($"Service {serviceType} has no implementation type.");

                var ctor = GetConstructorParameterTypesForMissingService(d);
                var args = ctor.Select(p => GetRequiredService(pluginType, p, 0)).ToArray();
                var result = Activator.CreateInstance(implementationType, args)!;
                var instancePool = _pluginScopedInstances[pluginType];
                instancePool.Add(new(serviceType, result));
                // Create instance
                return result;
            }
            else
            {
                Type implementationType =
                    d.ImplementationType ??
                    throw new InvalidOperationException($"Service {serviceType} has no implementation type.");

                var ctor = GetConstructorParameterTypesForMissingService(d);
                var args = ctor.Select(p => GetRequiredService(pluginType, p, 0)).ToArray();
                var result = Activator.CreateInstance(implementationType, args)!;
                return result;
            }
        }

    }
    public object CreateInstanceOfService(Type pluginType, Type serviceType, int layer)
    {
        if (layer == 1)
            return CreateInstanceOfServiceForRoot(pluginType, serviceType);
        else if (layer == 0)
            return CreateInstanceOfServiceForPlugin(pluginType, serviceType);
        else
            return GetHostService(serviceType);
    }

    private static IReadOnlyList<Type> GetConstructorParameterTypesForMissingService(ServiceDescriptor d)
    {
        var implType = d.ImplementationType;

        if (implType == null)
            return Array.Empty<Type>();

        var ctor = SelectConstructor(implType);
        return ctor.GetParameters().Select(p => p.ParameterType).ToArray();
    }

    private static ConstructorInfo SelectConstructor(Type implementationType)
    {
        var ctors = implementationType.GetConstructors(
            BindingFlags.Public | BindingFlags.Instance);

        if (ctors.Length == 0)
            throw new InvalidOperationException($"Type '{implementationType.FullName}' has no public constructors.");

        // Match Microsoft.Extensions.DependencyInjection behavior:
        // choose the constructor with the most parameters
        return ctors
            .OrderByDescending(c => c.GetParameters().Length)
            .First();
    }



}
