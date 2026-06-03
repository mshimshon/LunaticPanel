using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Circuit.Exceptions;
using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Abstraction.Tools.EventScheduler;
using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Core.CrazyReport;
using LunaticPanel.Core.Messaging;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.EventScheduledBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Core.PluginValidator;
using LunaticPanel.Core.Utils;
using LunaticPanel.Core.Utils.Logging;
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

    private readonly static Dictionary<PluginContextIdentifier, EventScheduledBusRegistry> _eventScheduledBusRegistry = new();
    private readonly static object _lockEventScheduledBusRegistry = new object();

    protected IServiceProvider? _crossCircuitSingletonProvider;
    private readonly string _pluginId;
    public string PluginId => _pluginId;
    private List<string> _internalKeys { get; set; }
    public IReadOnlyList<string> Keys { get; private set; }

    private bool _hasStarted;
    private List<BusHandlerDescriptor> _cacheBusHandlersDescriptors;
    protected PluginBase()
    {
        _pluginId = GetType().Namespace!;
        _internalKeys = GetMyPackageKeys().ToList();
        if (_cacheBusHandlersDescriptors == default)
            _cacheBusHandlersDescriptors = BusScannerExt.ScanBusHandlers(p => { }, GetPluginInternalAssemblies());
    }


    /* TODO: IMPLEMENT PRETECTIVE SYSTEM
     * PluginA.MyQuery; PluginB call PluginA.MyQuery; PluginA implements MyQuery
         [ Step 1: Prelist ] ────► Call Each Plugin.Keys this should return all the active keys.
                                        │
                                        ▼
         [ Step 2: Extract ] ────► Call string[] Plugin.CheckFeatureDegradation(Func<string,bool> checkForAvailableFeature)
                                        │
                                        ▼
         [ Step 3: Check ]   ────► Are all Used IDs present in the Prelist?
                                        │
                                        ├─── (Yes) ──► [ System is Stable & Ready ]
                                        │
                                 (No: Missing ID found)
                                        │
                                        ▼
         [ Step 4: Callback ] ───► Alert the specific plugin about its missing ID.
                                        │
                                        ▼
         [ Step 5: Disable ]  ───► Plugin returns a list of its OWN dependent IDs to disable.
                                        │
                                        ▼
         [ Step 6: Diff Check ] ─► Did the plugin actually return new IDs to remove?
                                        │
                                        ├─── (No/Empty) ─► Break loop (Prevents infinite loops).
                                        │
                                        └─── (Yes) ──────► Remove those specific IDs from the 
                                                           plugin's active registry, update 
                                                           the Prelist scope, and LOOP TO S
  */
    /// <summary>
    /// This is required if you have a package keys or using any bus event or queries or engine you should have a package anyway<br/>
    /// you should list all YOUR keys here or else the system will reject unregistered calls you can also use the extension<br/>
    /// typeof(BaseInfo).Assembly.ScanKeyPackageForKeys();
    /// </summary>
    /// <returns></returns>
    public abstract string[] GetMyPackageKeys();
    /// <summary>
    /// Two Parts
    /// 1. Remove from _cacheBusHandlersDescriptors which removes the Handler itself of the feature we disable.
    /// 2. Remove if available from Self Available Key List and Return the new list.
    /// disabling handlers for external events has no ramification for others only for ourselves
    /// </summary>
    protected void DisableBusFeature(string key)
    {
        key = key.ToLower();
        _cacheBusHandlersDescriptors = _cacheBusHandlersDescriptors
            .Where(p => !string.Equals(key, p.Key, StringComparison.OrdinalIgnoreCase))
            .ToList();
        _internalKeys = _internalKeys.Where(p => !string.Equals(key, p, StringComparison.OrdinalIgnoreCase)).ToList();
        Keys = _internalKeys.AsReadOnly();
    }

    public IReadOnlyList<string> CheckDependencyGracefully(Func<string, bool> isBusAvailable)
    {
        CheckFeatureDegradation(isBusAvailable);
        return Keys;
    }

    public abstract void CheckFeatureDegradation(Func<string, bool> isBusAvailable);

    public void SetScannedHandlersCache(string pluginId, Assembly[] toScan)
    {
        lock (_lockScannedCachedBusHandlers)
        {
            if (_scannedCachedBusHandlers.ContainsKey(pluginId))
                return;
            _scannedCachedBusHandlers[pluginId] = BusScannerExt.ScanBusHandlers(p => { }, toScan).AsReadOnly();
        }
    }
    public void OnCircuitStart(CircuitIdentity circuit)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        if (HasActiveCircuitFor(circuit.CircuitId)) return;

        SetScannedHandlersCache(PluginId, GetPluginInternalAssemblies());
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

        lock (_lockEventScheduledBusRegistry)
            if (!_eventScheduledBusRegistry.ContainsKey(identity))
                _eventScheduledBusRegistry[identity] = new();
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


        lock (_lockEventScheduledBusRegistry)
            if (_eventScheduledBusRegistry.ContainsKey(identity))
                _eventScheduledBusRegistry.Remove(identity);
    }



    public void OnCircuitEnd(CircuitIdentity circuit)
    {
        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        if (!HasActiveCircuitFor(circuit.CircuitId)) return;

        OnBeforeCircuitEnd(circuit);

        DeleteBusRegistry(circuit);

        lock (_circuitServiceProviders)
        {
            IServiceScope serviceScope = _circuitServiceProviders[identity];
            _circuitServiceProviders.Remove(identity);
            serviceScope.Dispose();
        }
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
        services.AddSingleton<IEventScheduledBusRegistry>((sp) =>
        {
            lock (_lockEventScheduledBusRegistry)
            {
                return _eventScheduledBusRegistry[identity];
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

        //services.AddPluginLocationUtilityService(PluginId);
        services.AddLunaticPanelUtilityServices(PluginId);

        //services.AddLinuxCommandUtilityService();
        //services.AddSafeFileWriterUtilityService();

        PluginContextIdentifier identity = new(circuit.CircuitId, PluginId);
        services.AddSingleton((sp) => _crossCircuitSingletonProvider!.GetRequiredService<IEngineBusRegistry>());
        services.AddSingleton((sp) => _crossCircuitSingletonProvider!.GetRequiredService<IEventBusRegistry>());
        services.AddSingleton((sp) => _crossCircuitSingletonProvider!.GetRequiredService<IQueryBusRegistry>());
        services.AddSingleton((sp) => _crossCircuitSingletonProvider!.GetRequiredService<IEventScheduledBusRegistry>());

        services.AddScoped<IPluginInfo>((sp) => this);
        services.AddScoped<EngineBus>();
        services.AddScoped<IEngineBus>((sp) => sp.GetRequiredService<EngineBus>());
        services.AddScoped<IEngineBusReceiver, EngineBusReceiver>();

        services.AddScoped<EventBus>();
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IEventBusReceiver, EventBusReceiver>();

        services.AddScoped<EventScheduledBus>();
        services.AddScoped<IEventScheduledBus, EventScheduledBus>();
        services.AddScoped<IEventScheduledBusReceiver, EventScheduledBusReceiver>();

        services.AddScoped<QueryBus>();
        services.AddScoped<IQueryBus>((sp) => sp.GetRequiredService<QueryBus>());
        services.AddScoped<IQueryBusReceiver, QueryBusReceiver>();

        services.AddScoped<ICrazyReportCircuit, CrazyReportCircuit>();
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


            foreach (BusHandlerDescriptor busInfo in cache)
            {
                if (busInfo.BusLifetime == EBusLifetime.Scoped)
                    services.AddScoped(busInfo.HandlerType);
                else
                    services.AddTransient(busInfo.HandlerType);


                if (busInfo.BusType == EBusType.EventBus)
                    lock (_lockEventBusRegistry)
                    {
                        _eventBusRegistry[identity].Register(busInfo.Key, busInfo);
                    }
                else if (busInfo.BusType == EBusType.QueryBus)
                    lock (_lockQueryBusRegistry)
                    {
                        _queryBusRegistry[identity].Register(busInfo.Key, busInfo);
                    }
                else if (busInfo.BusType == EBusType.EventScheduledBus)
                    lock (_lockEventScheduledBusRegistry)
                    {
                        _eventScheduledBusRegistry[identity].Register(busInfo.Key, busInfo);
                    }
                else
                    lock (_lockEngineBusRegistry)
                    {
                        _engineBusRegistry[identity].Register(busInfo.Key, busInfo);
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
    /// Called when the system begins checking the availability of each bus ID
    /// used by the plugin. If a required bus cannot be found or is unavailable,
    /// this method allows the plugin to disable its marker or degrade gracefully,
    /// preventing dependency‑based crashes caused by missing plugins or major
    /// changes in other plugins.
    /// </summary>
    protected virtual void OnBusIdMissings(IReadOnlyCollection<string> ids, Action<IReadOnlyCollection<string>> disableMineFor, CircuitIdentity circuit)
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
    /// Invoked before a circuit is ended, indicating that the client has disconnected.
    /// Plugins may use this hook to perform cleanup, release resources, or update while the Service provider is still alive but not UI attached
    /// circuit‑specific state associated with the disconnected client.
    /// </summary>
    protected virtual void OnBeforeCircuitEnd(CircuitIdentity circuit)
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

    public async Task BeforeRuntimeStartAsync(IServiceProvider serviceProvider)
    {
        if (_hasStarted)
        {
            Console.WriteLine($"BeforeRuntimeStart for {PluginId} already executed.");
            return;
        }
        _hasStarted = true;

        var circuitRegistry = serviceProvider.GetRequiredService<ICircuitRegistry>();
        var pContext = circuitRegistry.GetPluginContext(PluginId, circuitRegistry.CurrentCircuit.CircuitId);
        await BeforeRuntimeStart(pContext);
        await RegisterScheduledEventBus(pContext, circuitRegistry.CurrentCircuit);
    }

    private Task RegisterScheduledEventBus(IPluginContextService contextServices, CircuitIdentity circuit)
    {
        if (!circuit.IsMaster) return Task.CompletedTask;
        var evtSchService = contextServices.GetRequired<IEventScheduler>();
        var schEventRegistry = contextServices.GetRequired<IEventScheduledBusRegistry>();
        foreach (var d in schEventRegistry.GetAllAvailable())
        {
            if (!d.ScheduleAtStartup) continue;
            var t = new EventScheduleObject(d.Key, d.Timing)
            {
                RunOnceOnly = d.RunOnlyOnce
            };

            evtSchService.Register(t, d.RunAtStartup);
        }

        return Task.CompletedTask;
    }
}
