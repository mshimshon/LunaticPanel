namespace LunaticPanel.Core.Messaging.QuerySystem;

public interface IQueryBus
{
    IReadOnlyCollection<string> GetAllQueryIds();
    Task<QueryBusMessageResponse> QueryAsync(IQueryBusMessage qry);
}
