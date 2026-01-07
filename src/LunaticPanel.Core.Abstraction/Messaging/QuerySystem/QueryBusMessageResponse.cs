using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem.Exceptions;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public sealed record QueryBusMessageResponse
{
    public string Origin { get; init; } = default!;
    public BusMessageData? Data { get; init; }
    public QueryBusMessageException? Error { get; init; }
    public QueryBusMessageResponse(BusMessageData? data = default)
    {
        Data = data;
    }
}
