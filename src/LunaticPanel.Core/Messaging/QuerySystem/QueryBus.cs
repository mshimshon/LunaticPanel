using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Messaging.QuerySystem;

public sealed class QueryBus : IQueryBus, IQueryBusReceiver
{

    private readonly IServiceProvider _serviceProvider;
    private readonly IQueryBusRegistry _queryBusRegistry;
    private readonly IQueryBusExchange _queryBusExchange;

    public QueryBus(IServiceProvider serviceProvider, IQueryBusRegistry queryBusRegistry, IQueryBusExchange queryBusExchange)
    {
        _serviceProvider = serviceProvider;
        _queryBusRegistry = queryBusRegistry;
        _queryBusExchange = queryBusExchange;
    }

    private static Task<QueryBusMessageResponse> ExecuteHandler(IQueryBusMessage qry, IServiceProvider serviceProvider, Type handlerType)
    {
        var handler = serviceProvider.GetRequiredService(handlerType) as IQueryBusHandler;
        return handler!.HandleAsync(qry);
    }
    public IReadOnlyCollection<string> WhatDoYouListenFor() => _queryBusRegistry.GetAllAvailableIds();
    public bool DoYouListenTo(string key) => _queryBusRegistry.HasKey(key);

    public IReadOnlyCollection<string> GetAvailableKeys() => _queryBusRegistry.GetAllAvailableIds();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _queryBusExchange.AnyListenerFor(key);

    public async Task<QueryBusMessageResponse?> IncomingMessageAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default)
    {
        string id = qry.GetId();
        var registry = _queryBusRegistry;
        try
        {
            var handler = registry.GetRegistryFor(id);
            if (handler == default) return default;
            try
            {
                return await ExecuteHandler(qry, _serviceProvider, handler.HandlerType);
            }
            catch (QueryBusMessageException ex)
            {
                // TODO: OPEN TELEMETRY? OR CAP EVENT
                return new() { Error = ex, Origin = handler.HandlerType.FullName! };
            }
            catch (Exception ex)
            {
                return new() { Error = new("INTERNAL", ex.Message), Origin = handler.HandlerType.FullName! };
            }


        }
        catch (Exception ex)
        {
            return new() { Error = new("INTERNAL", ex.Message), Origin = id };
        }
    }
    public Task<QueryBusMessageResponse> QueryAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default)
        => _queryBusExchange.ExchangeAsync(qry, cancellationToken);

}
