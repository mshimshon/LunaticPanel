using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Extenstions;

public sealed record EngineBusMsgResponseWithData<TData>
{
    public string Origin { get; init; } = default!;
    public RenderFragment Render { get; init; } = default!;
    public TData Data { get; init; } = default!;
    public EngineBusMsgResponseWithData(TData data, RenderFragment render, string origin)
    {
        Data = data;
        Render = render;
        Origin = origin;
    }
}
