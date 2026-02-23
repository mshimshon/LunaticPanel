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

    protected IServiceProvider? _crossCircuitSingletonProvider;
    private readonly string _pluginId;
    public string PluginId => _pluginId;

    private bool _hasStarted;
    protected PluginBase()
    {
        _pluginId = GetType().Namespace!;
    }

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

        bool isSingletonCollectionInitialized = _crossCircuitSingletonProvider != default;
        ServiceCollection? singletonServices = isSingletonCollectionInitialized ? default : new();

        if (!isSingletonCollectionInitialized)
        {
            RegisterCommonSingletonServices(singletonServices!, circuit);
            RegisterPluginSingletonServices(singletonServices!, circuit);
        }

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
                finalServices.AddSingleton(item.ServiceType, (sp) => _crossCircuitSingletonProvider!.GetRequiredService(item.ServiceType));
            else
                finalServices.Add(item);
        }

        CompileHostRedirectedServices(circuit, ref finalServices);

        if (singletonServices != default)
            _crossCircuitSingletonProvider = singletonServices.BuildServiceProvider().CreateScope().ServiceProvider;

        var serviceProvider = finalServices.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        lock (_circuitServiceProviders)
            _circuitServiceProviders[identity] = scope;

        OnAfterCircuitStart(scope.ServiceProvider);
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
    private void RegisterCommonSingletonServices(IServiceCollection services, CircuitIdentity circuit)
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
        services.AddSingleton((sp) => _crossCircuitSingletonProvider!.GetRequiredService<IEngineBusRegistry>());
        services.AddSingleton((sp) => _crossCircuitSingletonProvider!.GetRequiredService<IEventBusRegistry>());
        services.AddSingleton((sp) => _crossCircuitSingletonProvider!.GetRequiredService<IQueryBusRegistry>());
        services.AddScoped<EngineBus>();
        services.AddScoped<IEngineBus>((sp) => sp.GetRequiredService<EngineBus>());
        services.AddScoped<IEngineBusReceiver, EngineBusReceiver>();


        services.AddScoped<EventBus>();
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IEventBusReceiver, EventBusReceiver>();
        services.AddScoped<IPluginConfiguration>((sp) => new PluginConfiguration(PluginId));

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
    /// Registers any additional services required by the plugin, including services
    /// provided by external packages or libraries. This method is invoked each time
    /// a new circuit (client connection) is created. Plugins must avoid performing
    /// assembly scanning or other expensive operations here, as this would introduce
    /// significant runtime overhead.
    /// </summary>
    protected virtual void RegisterPluginServices(IServiceCollection services, CircuitIdentity circuit)
    {

    }


    /// <summary>
    /// <para>
    /// Singleton services intended to be shared across all circuits must be registered twice.
    /// </para>
    /// <para>
    /// First, in the global singleton pool, where the actual instance is created.
    /// </para>
    /// <para>
    /// Second, in each circuit service collection, as a forwarding registration that resolves
    /// the instance from the global singleton pool.
    /// </para>
    /// <para>
    /// This allows the service to be injected normally through the circuit IServiceProvider
    /// while still guaranteeing a single shared instance across all circuits.
    /// </para>
    /// <para>Usage:</para>
    /// <code>
    /// // Global singleton pool (inside your RegisterPluginSingletonServices)
    /// services.AddSingleton&lt;IEngineBusRegistry, EngineBusRegistry&gt;();
    ///
    /// // Circuit pool forwarding (inside your RegisterPluginServices)
    /// services.AddSingleton(sp => _singletonProvider!.GetRequiredService&lt;IEngineBusRegistry&gt;());
    /// </code>
    /// </summary>
    protected virtual void RegisterPluginSingletonServices(IServiceCollection services, CircuitIdentity circuit)
    {
    }

    /// <summary>
    /// Returns the set of assemblies that should be scanned for this plugin.
    /// By default, only the plugin's own assembly is included. If the plugin
    /// relies on additional Razor Class Libraries or other internal assemblies,
    /// they should be added to the returned array alongside the primary assembly.
    /// </summary>
    protected virtual Assembly[] GetPluginInternalAssemblies() { return [GetType().Assembly]; }

    /// <summary>
    /// Loads the plugin's boot‑time configuration from the application's appsettings.json.
    /// Administrators may provide a configuration section for this plugin, identified by its
    /// plugin ID. Plugins must treat all values as optional and fall back to their own
    /// internal defaults when entries are missing or empty.
    /// </summary>
    protected virtual void LoadConfiguration(IConfiguration configuration) { }

    /// <summary>
    /// Executes after all services have been registered and the application has been built,
    /// but before the runtime (e.g., Before Blazor) becomes active. A scoped service provider is
    /// created and passed to the plugin so it can perform initialization, load resources,
    /// and apply configuration within a scoped context for example singletons or other persistent settings.
    /// </summary>
    protected virtual Task BeforeRuntimeStart(IPluginContextService pluginContext)
    => Task.CompletedTask;

    /// <summary>
    /// Invoked after the initial circuit setup and framework boilerplate have completed
    /// when a client connects. Plugins may use this hook to perform per‑circuit
    /// initialization, resolve scoped services, or prepare any state needed for the
    /// newly established client session.
    /// </summary>
    protected virtual void OnAfterCircuitStart(IServiceProvider serviceProvider)
    {

    }

    /// <summary>
    /// Invoked after a circuit has ended, indicating that the client has disconnected.
    /// Plugins may use this hook to perform cleanup, release resources, or update
    /// circuit‑specific state associated with the disconnected client.
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

    public Task BeforeRuntimeStartAsync(IServiceProvider serviceProvider)
    {
        if (_hasStarted)
        {
            Console.WriteLine($"BeforeRuntimeStart for {PluginId} already executed.");
            return Task.CompletedTask;
        }
        _hasStarted = true;

        var circuitRegistry = serviceProvider.GetRequiredService<ICircuitRegistry>();
        var pContext = circuitRegistry.GetPluginContext(PluginId, circuitRegistry.CurrentCircuit.CircuitId);
        var t = BeforeRuntimeStart(pContext);
        return t;
    }
}
