using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public sealed record EngineBusResponse
{
    public string Origin { get; init; } = default!;
    public Type ComponentType { get; }
    public BusMessageData? Data { get; }
    public Guid Id { get; }
    public Func<bool>? VisibilityCondition { get; init; }

    public EngineBusResponse(Type componentType)
    {
        ComponentType = componentType;
        Id = Guid.NewGuid();

    }

    public EngineBusResponse(Type componentType, object data) : this(componentType)
    {
        Data = new(data);
    }

}
