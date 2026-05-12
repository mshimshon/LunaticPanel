namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public interface IQueryBusHandler
{
    Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry);
}
