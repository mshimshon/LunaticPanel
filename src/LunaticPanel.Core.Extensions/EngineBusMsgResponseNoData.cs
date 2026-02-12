using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Extensions;

public record EngineBusMsgResponseNoData
{
    public string Origin { get; init; }
    public Func<bool>? VisibilityCondition { get; init; }
    public RenderFragment? Render { get; }
    public Type? ComponentType { get; }
    public EngineBusMsgResponseNoData(RenderFragment render, string origin)
    {
        Render = render;
        Origin = origin;
    }

    public EngineBusMsgResponseNoData(Type componentType, string origin)
    {
        ComponentType = componentType;
        Origin = origin;
    }
}
