using LunaticPanel.Core.Messaging.Common;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Messaging.EngineBus;

public sealed record EngineBusResponse
{
    public string Origin { get; init; } = default!;
    public RenderFragment RenderFragment { get; }
    public BusMessageData? Data { get; }
    public EngineBusResponse(RenderFragment renderFragment)
    {
        RenderFragment = renderFragment;
    }

    public EngineBusResponse(RenderFragment renderFragment, object data) : this(renderFragment)
    {
        Data = new(data);
    }

}
