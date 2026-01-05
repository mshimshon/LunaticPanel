using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.QuerySystem;

public interface IQueryBus
{
    IReadOnlyCollection<string> GetAvailableKeys();
    Task<QueryBusMessageResponse> QueryAsync(IQueryBusMessage qry);
    bool HasKeyFor(MessageKey messageKey);
    bool HasKeyFor(string key);
}
