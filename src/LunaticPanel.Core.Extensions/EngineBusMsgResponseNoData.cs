using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Extensions;

public record EngineBusMsgResponseNoData
{
    public string Origin { get; init; }
    public RenderFragment Render { get; init; }
    public EngineBusMsgResponseNoData(RenderFragment render, string origin)
    {
        Render = render;
        Origin = origin;
    }
}
