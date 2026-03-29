using LunaticPanel.Core.Abstraction;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Engine.Application.Circuit.Contracts;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Web.Layout;

namespace LunaticPanel.Engine.Web.Services.Circuit;

public sealed class CircuitRegistry : ICircuitRegistry
{
    private static List<CircuitHostIdentity> _circuits = new();
    private static readonly object _lock = new();
    private readonly IPluginRegistry _pluginRegistry;
    private readonly IServiceProvider _hostSP;
    private static Dictionary<PluginContextIdentifier, IPluginContextService> PluginContexts { get; } = new();
    private static Dictionary<PluginContextIdentifier, CircuitPluginIdentity> PluginCircuitIdentity { get; } = new();

    private CircuitHostIdentity _currentCircuit = default!;
    public CircuitIdentity CurrentCircuit => _currentCircuit;
    private MainLayout? _app;
    private static readonly object _lockPluginContexts = new();
    public CircuitRegistry(IPluginRegistry pluginRegistry, IServiceProvider hostSP)
    {
        _pluginRegistry = pluginRegistry;
        _hostSP = hostSP;
    }
    public IReadOnlyCollection<CircuitIdentity> GetActiveCircuits()
    {
        lock (_lock)
        {
            return _circuits.Where(p => !ReferenceEquals(p.LayoutComponent, null) || p.IsMaster).ToList().AsReadOnly();
        }
    }
    internal void SelfCircuitRegistration(Guid id, MainLayout? app)
    {
        lock (_lock)
        {

            Console.WriteLine($"Is {id} MAster ? {(app == default)}");
            _currentCircuit = new()
            {
                CircuitId = id,
                LayoutComponent = app,
                HostServiceProvider = _hostSP,
                IsMaster = app == default
            };
            _circuits.Add(_currentCircuit);
        }
        var plugins = _pluginRegistry.GetAll();
        foreach (var item in plugins)
        {
            lock (_lockPluginContexts)
            {
                var circuit = new CircuitPluginIdentity()
                {
                    HostServiceProvider = _hostSP,
                    CircuitId = id,
                    PluginId = item.Plugin.Identity.PackageId.ToLower(),
                    Entry = item.Entry,
                    IsMaster =
                    _currentCircuit.IsMaster
                };
                circuit.Entry.OnCircuitStart(circuit);
                var identifier = new PluginContextIdentifier(circuit.CircuitId, circuit.PluginId);
                PluginContexts.Add(identifier, circuit.Entry.GetContext(circuit.CircuitId));
                PluginCircuitIdentity.Add(identifier, circuit);
            }

        }
    }

    /* TODO: FINISH THIS
     * We need a state scoped to each circuit for all plugins via host DI its statepulse state hidden behind an interface with the property and the interface is available
     * inside the abstraction packgage.
     * The interface provider a CountUp method, CountDown mehtod to increase and decrease the components count into the state.
     * The interface also has a event which we can subscribe to receive update of the state.
     * once we receive update it passe the count every single time once count is 0 we unsubscribe to the event and trigger SelfRemoval.
     */

    internal void SelfRemoval()
    {
        lock (_lock)
        {
            if (_currentCircuit != default && _currentCircuit.LayoutComponent == _app)
                _circuits.Remove(_currentCircuit);
        }
        if (_currentCircuit == default) return;
        List<CircuitPluginIdentity> identities;

        lock (_lockPluginContexts)
        {
            var keys = PluginContexts.Keys
                .Where(k => k.CircuitId == _currentCircuit.CircuitId)
                .ToList();

            identities = new List<CircuitPluginIdentity>(keys.Count);

            foreach (var key in keys)
            {
                identities.Add(PluginCircuitIdentity[key]);

                PluginContexts.Remove(key);
                PluginCircuitIdentity.Remove(key);
            }
        }

        foreach (var identity in identities)
        {
            identity.Entry.OnCircuitEnd(identity);
        }


    }

    public IPluginContextService GetPluginContext(string pluginId, Guid circuitId)
    {
        lock (_lockPluginContexts)
        {
            pluginId = pluginId.ToLower();
            return PluginContexts.Single(p => p.Key.PluginId == pluginId && p.Key.CircuitId == circuitId).Value;
        }

    }

    public IReadOnlyDictionary<PluginContextIdentifier, IPluginContextService> GetPluginContexts()
    {
        lock (_lockPluginContexts)
        {
            return PluginContexts.AsReadOnly();
        }
    }
    public IReadOnlyDictionary<PluginContextIdentifier, CircuitPluginIdentity> GetCircuitPluginIdentities()
    {
        lock (_lockPluginContexts)
        {
            return PluginCircuitIdentity.AsReadOnly();
        }
    }
}



