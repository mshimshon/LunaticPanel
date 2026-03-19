using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

namespace LunaticPanel.Core.Extensions;

public static class QueryBusExt
{
    public static Task<QueryBusMessageResponse> ReplyWith(this IQueryBusMessage queryBusMessage, object? data)
    {
        return Task.FromResult(new QueryBusMessageResponse(data != default ? new(data) : default));
    }
    public static async Task<QueryBusMessageResponse> QueryWithoutDataAsync(this IQueryBus qryBus, string key)
    => await qryBus.QueryAsync(new QueryBusMessage(new(key)));
    public static async Task<QueryBusMessageResponse> QueryWithDataAsync(this IQueryBus qryBus, string key, object data)
        => await qryBus.QueryAsync(new QueryBusMessage(new(key), data));

    public static async Task<QueryBusMessageResponse> QueryWithoutDataAsync(this IQueryBus qryBus, string key, Guid targetCircuit)
        => await qryBus.QueryAsync(new QueryBusMessage(new(key))
        {
            TargetCircuit = targetCircuit
        });
    public static async Task<QueryBusMessageResponse> QueryWithDataAsync(this IQueryBus qryBus, string key, object data, Guid targetCircuit)
        => await qryBus.QueryAsync(new QueryBusMessage(new(key), data)
        {
            TargetCircuit = targetCircuit
        });
    public static Task<TData?> ReadAs<TData>(this QueryBusMessageResponse response)
    {
        var data = response.Data;
        if (data == default)
            return Task.FromResult<TData?>(default);
        var result = data.GetDataAs<TData>();
        return Task.FromResult(result);
    }
    public static Task<TData?> ReadAs<TData>(this IQueryBusMessage qryMsg)
    {
        var data = qryMsg.GetData();
        if (data == default)
            return Task.FromResult<TData?>(default);
        var result = data.GetDataAs<TData>();
        return Task.FromResult(result);
    }
}
