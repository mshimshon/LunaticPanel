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
    private readonly static Dictionary<Guid, IServiceScope> _circuitServiceProviders = new();
    private static IReadOnlyCollection<BusHandlerDescriptor>? _scannedCachedBusHandlers;
    private readonly static object _lockCircuitRegistry = new object();
    private List<HostRedirectionService> _hostRedirectedServices = new();
    private readonly static EventBusRegistry _eventBusRegistry = new();
    private readonly static QueryBusRegistry _queryBusRegistry = new();
    private readonly static EngineBusRegistry _engineBusRegistry = new();
    private IServiceProvider? _singletonProvider;
    private readonly string _pluginId;
    public string PluginId => _pluginId;

    protected PluginBase() { _pluginId = GetType().Namespace!; }

    private static void SetScannedHandlersCache(Assembly[] toScan)
    {
        lock (_lockCircuitRegistry)
        {
            _scannedCachedBusHandlers = BusScannerExt.ScanBusHandlers(toScan).AsReadOnly();
        }
    }
    public void OnCircuitStart(CircuitIdentity circuit)
    {
        if (HasActiveCircuitFor(circuit.CircuitId)) return;

        if (_scannedCachedBusHandlers == default)
            SetScannedHandlersCache(GetPluginInternalAssemblies());

        var allServices = new ServiceCollection();
        RegisterCommonServices(allServices, circuit);
        RegisterPluginServices(allServices, circuit);

        bool isSingletonCollectionInitialized = _singletonProvider != default;
        ServiceCollection? singletonServices = isSingletonCollectionInitialized ? default : new();
        var finalServices = new ServiceCollection();
        foreach (var item in allServices)
        {
            if (!isSingletonCollectionInitialized && item.Lifetime == ServiceLifetime.Singleton)
                singletonServices!.Add(item);

            if (item.Lifetime == ServiceLifetime.Singleton)
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
        {
            _circuitServiceProviders[circuit.CircuitId] = scope;
        }

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
    public void OnCircuitEnd(CircuitIdentity circuit)
    {
        if (!HasActiveCircuitFor(circuit.CircuitId)) return;
        IServiceScope serviceScope;
        lock (_circuitServiceProviders)
        {
            serviceScope = _circuitServiceProviders[circuit.CircuitId];
            _circuitServiceProviders.Remove(circuit.CircuitId);
        }
        serviceScope.Dispose();
        OnAfterCircuitEnd(circuit);
    }
    public IPluginContextService GetContext(Guid circuitId)
    {
        if (!HasActiveCircuitFor(circuitId))
            throw new CircuitClosedException(circuitId);
        IServiceScope serviceScope;
        lock (_lockCircuitRegistry)
        {
            var result = _circuitServiceProviders.Single(p => p.Key == circuitId);
            serviceScope = result.Value;
        }
        return serviceScope.ServiceProvider.GetRequiredService<IPluginContextService>();
    }
    public void Configure(IConfiguration configuration) => LoadConfiguration(configuration);
    protected static bool HasActiveCircuitFor(Guid id)
    {
        lock (_lockCircuitRegistry)
        {
            return _circuitServiceProviders.Any(p => p.Key == id);
        }
    }

    private void RegisterCommonServices(IServiceCollection services, CircuitIdentity circuit)
    {

        services.AddSingleton<IEngineBusRegistry>((sp) => _engineBusRegistry);
        services.AddScoped<EngineBus>();
        services.AddScoped<IEngineBus>((sp) => sp.GetRequiredService<EngineBus>());
        services.AddScoped<IEngineBusReceiver, EngineBusReceiver>();

        services.AddSingleton<IEventBusRegistry>((sp) => _eventBusRegistry);
        services.AddScoped<EventBus>();
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IEventBusReceiver, EventBusReceiver>();

        services.AddSingleton<IQueryBusRegistry>((sp) => _queryBusRegistry);
        services.AddScoped<QueryBus>();
        services.AddScoped<IQueryBus>((sp) => sp.GetRequiredService<QueryBus>());
        services.AddScoped<IQueryBusReceiver, QueryBusReceiver>();

        services.AddScoped(sp => new PluginContext(sp, circuit));
        services.AddScoped<IPluginContext>(sp => sp.GetRequiredService<PluginContext>());
        services.AddScoped<IPluginContextService>(sp => sp.GetRequiredService<PluginContext>());
        services.AddScoped<IWidgetContext>(sp => sp.GetRequiredService<PluginContext>());

        if (_scannedCachedBusHandlers != default)
            foreach (var item in _scannedCachedBusHandlers)
            {
                services.AddTransient(item.HandlerType);
                if (item.BusType == EBusType.EventBus)
                    _eventBusRegistry.Register(item.Id, item);
                else if (item.BusType == EBusType.QueryBus)
                    _queryBusRegistry.Register(item.Id, item);
                else
                    _engineBusRegistry.Register(item.Id, item);
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
