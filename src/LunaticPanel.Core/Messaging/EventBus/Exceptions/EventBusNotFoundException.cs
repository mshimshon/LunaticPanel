namespace LunaticPanel.Core.Messaging.EventBus.Exceptions;

public class EventBusNotFoundException : Exception
{
    public string Id { get; }
    public EventBusNotFoundException(string id) : base($"({id}) is not a registered event.")
    {
        Id = id;
    }
}
