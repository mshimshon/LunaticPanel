using LunaticPanel.Engine.Application.Circuit.Contracts;

namespace LunaticPanel.Engine.Application.Circuit;

public interface ICircuitControl
{
    IReadOnlyCollection<CircuitIdentityDto> GetActiveCircuits();
}
