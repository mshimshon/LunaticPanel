namespace LunaticPanel.Engine.Application.Circuit.Contracts;

public sealed record CircuitIdentityDto
{
    public Guid Id { get; set; }
    public object App { get; set; }
    public Func<IServiceProvider> ServiceProvider { get; set; } = default!;
}
