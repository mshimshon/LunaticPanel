namespace GameServerManager.Engine.Domain.Messaging.Entities;

public sealed record EventTypeEntity
{
    public Type HandlerType { get; init; } = default!;
    public bool IsCrossCircuitType { get; init; }
}
