namespace LunaticPanel.Engine.Infrastructure.Circuit;

public interface ICircuitControl
{
    IReadOnlyCollection<CircuitIdentityDto> GetActiveCircuits();
}
