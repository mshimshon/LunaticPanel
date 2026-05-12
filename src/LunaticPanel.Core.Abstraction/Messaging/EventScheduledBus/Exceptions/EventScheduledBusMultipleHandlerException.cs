namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus.Exceptions;

public class EventScheduledBusMultipleHandlerException : Exception
{
    public string Id { get; }
    public EventScheduledBusMultipleHandlerException(string id) : base($"({id}) is already registered.")
    {
        Id = id;
    }
}
