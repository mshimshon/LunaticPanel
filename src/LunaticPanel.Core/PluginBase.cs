using LunaticPanel.Core.Abstraction;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Circuit.Exceptions;
using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Messaging;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Core.PluginValidator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace LunaticPanel.Core;

public abstract class PluginBase : IPlugin
{
    private readonly static Dictionary<PluginContextIdentifier, IServiceScope> _circuitServiceProviders = new();
    private readonly static object _lockCircuitRegistry = new object();

    private readonly static Dictionary<string, IReadOnlyCollection<BusHandlerDescriptor>> _scannedCachedBusHandlers = new();
    private readonly static object _lockScannedCachedBusHandlers = new object();

    private List<HostRedirectionService> _hostRedirectedServices = new();
    private readonly static Dictionary<PluginContextIdentifier, EventBusRegistry> _eventBusRegistry = new();
    private readonly static object _lockEventBusRegistry = new object();

    private readonly static Dictionary<PluginContextIdentifier, QueryBusRegistry> _queryBusRegistry = new();
    private readonly static object _lockQueryBusRegistry = new object();

    private readonly static Dictionary<PluginContextIdentifier, EngineBusRegistry> _engineBusRegistry = new();
    private readonly static object _lockEngineBusRegistry = new object();

    private IServiceProvider? _singletonProvider;
    private readonly string _pluginId;
    public string PluginId => _pluginId;
    protected PluginBase() { _pluginId = GetType().Namespace!; }

    private void SetScannedHandlersCache(CircuitIdentity circuit, Assembly[] toScan)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        lock (_lockScannedCachedBusHandlers)
        {
            if (_scannedCachedBusHandlers.ContainsKey(identity.PluginId))
                return;
            _scannedCachedBusHandlers[identity.PluginId] = BusScannerExt.ScanBusHandlers(toScan).AsReadOnly();
        }
    }
    public void OnCircuitStart(CircuitIdentity circuit)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        if (HasActiveCircuitFor(circuit.CircuitId)) return;

        SetScannedHandlersCache(circuit, GetPluginInternalAssemblies());
        CreateBusRegistry(circuit);

        var allServices = new ServiceCollection();
        RegisterCommonServices(allServices, circuit);
        RegisterPluginServices(allServices, circuit);

        bool isSingletonCollectionInitialized = _singletonProvider != default;
        ServiceCollection? singletonServices = isSingletonCollectionInitialized ? default : new();

        if (!isSingletonCollectionInitialized)
            RegisterSingletonServices(singletonServices!, circuit);

        var finalServices = new ServiceCollection();
        foreach (var item in allServices)
        {
            bool isGlobalState = false;
            if (!isSingletonCollectionInitialized && item.Lifetime == ServiceLifetime.Singleton)
            {
                isGlobalState = item.ServiceType.GetInterfaces().Any(i => i.FullName == "StatePulse.Net.IStateFeatureSingleton");
                if (isGlobalState)
                    singletonServices!.Add(item);
            }

            if (isGlobalState && item.Lifetime == ServiceLifetime.Singleton)
                finalServices.AddSingleton(item.ServiceType, (sp) => _singletonProvider!.GetRequiredService(item.ServiceType));
            else
                finalServices.Add(item);
        }

        CompileHostRedirectedServices(circuit, ref finalServices);

        if (singletonServices != default)
            _singletonProvider = singletonServices.BuildServiceProvider().CreateScope().ServiceProvider;

        var serviceProvider = finalServices.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        lock (_circuitServiceProviders)
            _circuitServiceProviders[identity] = scope;
    }

    private void CreateBusRegistry(CircuitIdentity circuit)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        lock (_lockEventBusRegistry)
            if (!_eventBusRegistry.ContainsKey(identity))
                _eventBusRegistry[identity] = new();

        lock (_lockQueryBusRegistry)
            if (!_queryBusRegistry.ContainsKey(identity))
                _queryBusRegistry[identity] = new();

        lock (_lockEngineBusRegistry)
            if (!_engineBusRegistry.ContainsKey(identity))
                _engineBusRegistry[identity] = new();
    }
    public void AddHostRedirectedServices(params HostRedirectionService[] serviceTypes)
    {
        lock (_lockCircuitRegistry)
        {
            if (_hostRedirectedServices == default)
                _hostRedirectedServices = new List<HostRedirectionService>();
            _hostRedirectedServices.AddRange(serviceTypes);
        }
    }
    public void CompileHostRedirectedServices(CircuitIdentity circuit, ref ServiceCollection result)
    {
        if (_hostRedirectedServices == default) return;
        foreach (var item in _hostRedirectedServices)
        {
            if (item.ServiceType.IsGenericTypeDefinition) continue;
            if (item.Lifetime == ServiceLifetime.Singleton)
                result.AddSingleton(item.ServiceType, (sp) =>
                {
                    return circuit.HostServiceProvider.GetRequiredService(item.ServiceType);
                });
            else if (item.Lifetime == ServiceLifetime.Scoped)
                result.AddScoped(item.ServiceType, (sp) => circuit.HostServiceProvider.GetRequiredService(item.ServiceType));
            else if (item.Lifetime == ServiceLifetime.Transient)
                result.AddTransient(item.ServiceType, (sp) => circuit.HostServiceProvider.GetRequiredService(item.ServiceType));
        }
    }

    private void DeleteBusRegistry(CircuitIdentity circuit)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        lock (_lockEventBusRegistry)
            if (_eventBusRegistry.ContainsKey(identity))
                _eventBusRegistry.Remove(identity);

        lock (_lockQueryBusRegistry)
            if (_queryBusRegistry.ContainsKey(identity))
                _queryBusRegistry.Remove(identity);

        lock (_lockEngineBusRegistry)
            if (_engineBusRegistry.ContainsKey(identity))
                _engineBusRegistry.Remove(identity);
    }

    public void OnCircuitEnd(CircuitIdentity circuit)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        if (!HasActiveCircuitFor(circuit.CircuitId)) return;

        DeleteBusRegistry(circuit);

        IServiceScope serviceScope;
        lock (_circuitServiceProviders)
        {
            serviceScope = _circuitServiceProviders[identity];
            _circuitServiceProviders.Remove(identity);
        }
        serviceScope.Dispose();
        OnAfterCircuitEnd(circuit);
    }

    public IPluginContextService GetContext(Guid circuitId)
    {
        PluginContextIdentifier identity = new(circuitId, PluginId);
        if (!HasActiveCircuitFor(circuitId))
            throw new CircuitClosedException(circuitId);
        IServiceScope serviceScope;
        lock (_lockCircuitRegistry)
        {
            var result = _circuitServiceProviders[identity];
            serviceScope = result;
        }
        return serviceScope.ServiceProvider.GetRequiredService<IPluginContextService>();
    }
    public void Configure(IConfiguration configuration) => LoadConfiguration(configuration);
    protected bool HasActiveCircuitFor(Guid circuitId)
    {
        PluginContextIdentifier identity = new(circuitId, PluginId);
        lock (_lockCircuitRegistry)
        {
            return _circuitServiceProviders.ContainsKey(identity);
        }
    }
    private void RegisterSingletonServices(IServiceCollection services, CircuitIdentity circuit)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        services.AddSingleton<IEngineBusRegistry>((sp) =>
        {
            lock (_lockEngineBusRegistry)
            {
                return _engineBusRegistry[identity];
            }
        });
        services.AddSingleton<IEventBusRegistry>((sp) =>
        {
            lock (_lockEventBusRegistry)
            {
                return _eventBusRegistry[identity];
            }
        });
        services.AddSingleton<IQueryBusRegistry>((sp) =>
        {
            lock (_lockQueryBusRegistry)
            {
                return _queryBusRegistry[identity];
            }
        });
    }

    private void RegisterCommonServices(IServiceCollection services, CircuitIdentity circuit)
    {

        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        services.AddSingleton((sp) => _singletonProvider!.GetRequiredService<IEngineBusRegistry>());
        services.AddSingleton((sp) => _singletonProvider!.GetRequiredService<IEventBusRegistry>());
        services.AddSingleton((sp) => _singletonProvider!.GetRequiredService<IQueryBusRegistry>());
        services.AddScoped<EngineBus>();
        services.AddScoped<IEngineBus>((sp) => sp.GetRequiredService<EngineBus>());
        services.AddScoped<IEngineBusReceiver, EngineBusReceiver>();


        services.AddScoped<EventBus>();
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IEventBusReceiver, EventBusReceiver>();


        services.AddScoped<QueryBus>();
        services.AddScoped<IQueryBus>((sp) => sp.GetRequiredService<QueryBus>());
        services.AddScoped<IQueryBusReceiver, QueryBusReceiver>();

        services.AddScoped(sp => new PluginContext(sp, circuit));
        services.AddScoped<IPluginContext>(sp => sp.GetRequiredService<PluginContext>());
        services.AddScoped<IPluginContextService>(sp => sp.GetRequiredService<PluginContext>());
        services.AddScoped<IWidgetContext>(sp => sp.GetRequiredService<PluginContext>());
        bool hasAlreadyScannedForBus;
        lock (_lockScannedCachedBusHandlers)
        {
            hasAlreadyScannedForBus = _scannedCachedBusHandlers.ContainsKey(identity.PluginId);
        }
        if (hasAlreadyScannedForBus)
        {
            IReadOnlyCollection<BusHandlerDescriptor> cache;
            lock (_lockScannedCachedBusHandlers)
                cache = _scannedCachedBusHandlers[identity.PluginId];


            foreach (var item in cache)
            {
                services.AddTransient(item.HandlerType);
                if (item.BusType == EBusType.EventBus)
                    lock (_lockEventBusRegistry)
                    {
                        _eventBusRegistry[identity].Register(item.Id, item);
                    }
                else if (item.BusType == EBusType.QueryBus)
                    lock (_lockQueryBusRegistry)
                    {

                        _queryBusRegistry[identity].Register(item.Id, item);
                    }

                else
                    lock (_lockEngineBusRegistry)
                    {

                        _engineBusRegistry[identity].Register(item.Id, item);
                    }
            }
        }

    }

    /// <summary>
    /// Register any extra services maybe from other package you may be using.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="circuit"></param>
    protected virtual void RegisterPluginServices(IServiceCollection services, CircuitIdentity circuit)
    {

    }

    /// <summary>
    /// By default this will only tag your own plugin assembly to be scanned if you have additional razor class library add them along side your plugin assembly.
    /// </summary>
    /// <returns></returns>
    protected virtual Assembly[] GetPluginInternalAssemblies() { return [GetType().Assembly]; }

    /// <summary>
    /// This is called a boot time... this is the first thing plugin get hit by.
    /// All plugin will get it own section plugin id from the appSetting.Json if present.
    /// </summary>
    protected virtual void LoadConfiguration(IConfiguration configuration) { }

    /// <summary>
    /// This is called after initial boilerplate are done when client connects
    /// </summary>
    protected virtual void OnAfterCircuitStart(IServiceProvider serviceProvider)
    {

    }

    /// <summary>
    /// This is called after a circuit is closed (client disconnected)
    /// </summary>
    protected virtual void OnAfterCircuitEnd(CircuitIdentity circuit)
    {

    }

    private IReadOnlyCollection<PluginValidationResult>? _passValidation;
    public IReadOnlyCollection<PluginValidationResult> PerformValidation()
    {
        if (_passValidation != default)
            return _passValidation;
        List<PluginValidationResult> resultToReturn = [
                this.FindAnyInvalidRoutesNames(),
                this.FindAnyWidgetNotUsingProperComponentBase()
            ];
        _passValidation = resultToReturn.AsReadOnly();
        return PerformValidation();
    }
}
