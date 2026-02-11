using LunaticPanel.Core.Abstraction.Messaging.Common;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public sealed record EngineBusResponse
{
    public string Origin { get; init; } = default!;
    public RenderFragment RenderFragment { get; }
    public BusMessageData? Data { get; }
    public Guid Id { get; }
    public Func<bool>? VisibilityCondition { get; init; }
    public EngineBusResponse(RenderFragment renderFragment)
    {
        RenderFragment = renderFragment;
        Id = Guid.NewGuid();
    }

    public EngineBusResponse(RenderFragment renderFragment, object data) : this(renderFragment)
    {
        Data = new(data);
    }

}
