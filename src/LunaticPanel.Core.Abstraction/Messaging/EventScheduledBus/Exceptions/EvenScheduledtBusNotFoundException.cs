namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus.Exceptions;

public class EvenScheduledtBusNotFoundException : Exception
{
    public string Id { get; }
    public EvenScheduledtBusNotFoundException(string id) : base($"({id}) is not a registered event.")
    {
        Id = id;
    }
}
