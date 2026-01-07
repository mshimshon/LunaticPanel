using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public interface IQueryBus
{
    IReadOnlyCollection<string> GetAvailableKeys();
    Task<QueryBusMessageResponse> QueryAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default);
    bool HasKeyFor(MessageKey messageKey);
    bool HasKeyFor(string key);
}
