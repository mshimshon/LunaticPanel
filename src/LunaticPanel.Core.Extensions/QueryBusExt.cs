using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

namespace LunaticPanel.Core.Extensions;

public static class QueryBusExt
{
    public static Task<QueryBusMessageResponse> ReplyWith(this IQueryBusMessage queryBusMessage, object data)
    {
        return Task.FromResult(new QueryBusMessageResponse(new(data)));
    }
}
