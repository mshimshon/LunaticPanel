namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem.Exceptions;

public class QueryBusNotFoundException : Exception
{
    public string Id { get; }
    public QueryBusNotFoundException(string id) : base($"({id}) is not a registered event.")
    {
        Id = id;
    }
}