namespace LunaticPanel.Core.Abstraction.Circuit;

public record CircuitIdentity
{

    public Guid CircuitId { get; set; }
    public IServiceProvider HostServiceProvider { get; set; } = default!;
    public bool IsMaster { get; set; }

}
