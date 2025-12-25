namespace LunaticPanel.Core.Messaging.QuerySystem.Exceptions;

public class QueryBusMultipleHandlerException : Exception
{
    public string Id { get; }
    public QueryBusMultipleHandlerException(string id) : base($"({id}) is already registered.")
    {
        Id = id;
    }
}
