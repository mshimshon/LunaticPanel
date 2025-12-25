namespace LunaticPanel.Core.Messaging.QuerySystem;

public interface IQueryBusHandler
{
    Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry);
}
