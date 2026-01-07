namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public interface IQueryBusExchange
{
    IReadOnlyCollection<string> GetAvailableKeys();
    Task<QueryBusMessageResponse> ExchangeAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default);
    bool AnyListenerFor(string key);
}
