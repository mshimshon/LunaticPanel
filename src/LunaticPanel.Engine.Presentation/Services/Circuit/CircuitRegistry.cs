using LunaticPanel.Core.Abstraction;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Engine.Application.Circuit.Contracts;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Presentation.Layout;

namespace LunaticPanel.Engine.Web.Services.Circuit;

public sealed class CircuitRegistry : ICircuitRegistry
{
    private static List<CircuitHostIdentity> _circuits = new();
    private static readonly object _lock = new();
    private readonly IPluginRegistry _pluginRegistry;
    private readonly IServiceProvider _hostSP;
    private CircuitHostIdentity _currentCircuit = default!;
    private static Dictionary<PluginContextIdentifier, IPluginContextService> PluginContexts { get; } = new();
    private static Dictionary<PluginContextIdentifier, CircuitPluginIdentity> PluginCircuitIdentity { get; } = new();

    public CircuitIdentity CurrentCircuit => _currentCircuit;

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
            return _circuits.Where(p => !ReferenceEquals(p.LayoutComponent, null)).ToList().AsReadOnly();
        }
    }
    internal void SelfCircuitRegistration(Guid id, MainLayout app)
    {
        lock (_lock)
        {
            _currentCircuit = new() { CircuitId = id, LayoutComponent = app, HostServiceProvider = app.ServiceProvider };
            _circuits.Add(_currentCircuit);
        }
        foreach (var item in _pluginRegistry.GetAll())
        {
            lock (_lockPluginContexts)
            {
                var circuit = new CircuitPluginIdentity()
                {
                    HostServiceProvider = _hostSP,
                    CircuitId = id,
                    PluginId = item.Plugin.Identity.PackageId.ToLower(),
                    Entry = item.Entry
                };
                circuit.Entry.OnCircuitStart(circuit);
                var identifier = new PluginContextIdentifier(circuit.CircuitId, circuit.PluginId);
                PluginContexts.Add(identifier, circuit.Entry.GetContext(circuit.CircuitId));
                PluginCircuitIdentity.Add(identifier, circuit);
            }

        }
    }
    internal void SelfRemoval(Guid id, MainLayout app)
    {
        lock (_lock)
        {
            if (_currentCircuit != default && _currentCircuit.LayoutComponent == app)
                _circuits.Remove(_currentCircuit);
        }
        List<CircuitPluginIdentity> identities;

        lock (_lockPluginContexts)
        {
            var keys = PluginContexts.Keys
                .Where(k => k.CircuitId == id)
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



