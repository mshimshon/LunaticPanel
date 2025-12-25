namespace LunaticPanel.Engine.Infrastructure.Circuit;

public sealed record CircuitIdentityDto
{
    public Guid Id { get; set; }
    public object App { get; set; }
    public Func<IServiceProvider> ServiceProvider { get; set; } = default!;
}
