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

    public static async Task<TResult?> QueryWithoutDataAsync<TResult>(this IQueryBus qryBus, string key)
    {
        var response = await qryBus.QueryWithoutDataAsync(key);
        return await response.ReadAs<TResult>();
    }
    public static async Task<TResult?> QueryWithDataAsync<TResult>(this IQueryBus qryBus, string key, object data)
    {
        var response = await qryBus.QueryWithDataAsync(key, data);
        return await response.ReadAs<TResult>();
    }

    public static async Task<TResult?> QueryWithoutDataAsync<TResult>(this IQueryBus qryBus, string key, Guid targetCircuit)
    {
        var response = await qryBus.QueryWithDataAsync(key, targetCircuit);
        return await response.ReadAs<TResult>();
    }
    public static async Task<TResult?> QueryWithDataAsync<TResult>(this IQueryBus qryBus, string key, object data, Guid targetCircuit)
    {
        var response = await qryBus.QueryWithDataAsync(key, data, targetCircuit);
        return await response.ReadAs<TResult>();
    }
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
