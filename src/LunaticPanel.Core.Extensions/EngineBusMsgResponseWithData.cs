using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Extensions;

public record EngineBusMsgResponseWithData<TData> : EngineBusMsgResponseNoData
{
    public TData Data { get; init; }
    public EngineBusMsgResponseWithData(TData data, RenderFragment render, string origin) : base(render, origin)
    {
        Data = data;
    }
}
