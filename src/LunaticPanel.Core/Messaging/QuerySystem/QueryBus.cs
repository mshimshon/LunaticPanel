using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

namespace LunaticPanel.Core.Messaging.QuerySystem;

public sealed class QueryBus : IQueryBus
{

    private readonly IQueryBusExchange _queryBusExchange;

    public QueryBus(IQueryBusExchange queryBusExchange)
    {
        _queryBusExchange = queryBusExchange;
    }


    public IReadOnlyCollection<string> GetAvailableKeys() => _queryBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _queryBusExchange.AnyListenerFor(key);


    public Task<QueryBusMessageResponse> QueryAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default)
        => _queryBusExchange.ExchangeAsync(qry, cancellationToken);

}
