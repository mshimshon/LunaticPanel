namespace LunaticPanel.Core.Abstraction.Circuit.Exceptions;

public class CircuitClosedException : Exception
{
    public CircuitClosedException(Guid circuitId) : base($"The circuit {circuitId} was closed")
    {
    }
}
