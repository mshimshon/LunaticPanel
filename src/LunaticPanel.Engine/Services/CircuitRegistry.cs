using LunaticPanel.Engine.Application.Circuit;
using LunaticPanel.Engine.Application.Circuit.Contracts;
using LunaticPanel.Engine.Layout;

namespace LunaticPanel.Engine.Presentation.Services;

public sealed class CircuitRegistry : ICircuitControl
{
    private static List<CircuitIdentityDto> _circuits = new();
    private static readonly object _lock = new();
    private CircuitIdentityDto _currentCircuit = default!;


    public IReadOnlyCollection<CircuitIdentityDto> GetActiveCircuits()
    {
        lock (_lock)
        {
            return _circuits.Where(p => !ReferenceEquals(p.App, null)).ToList().AsReadOnly();
        }
    }
    internal void SelfCircuitRegistration(Guid id, MainLayout app)
    {
        lock (_lock)
        {
            _currentCircuit = new() { Id = id, App = app, ServiceProvider = () => app.ServiceProvider };
            _circuits.Add(_currentCircuit);
        }
    }
    internal void SelfRemoval(Guid id, MainLayout app)
    {
        lock (_lock)
        {
            if (_currentCircuit != default && _currentCircuit.App == app)
                _circuits.Remove(_currentCircuit);
        }
    }
}



