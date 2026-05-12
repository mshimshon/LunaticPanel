using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus.Exceptions;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public sealed record EventScheduledBusMessageResponse
{
    public string Origin { get; init; } = default!;
    public EventScheduledBusMessageData Data { get; }
    public EventScheduledBusMessageException? Error { get; init; }
    public Guid Id { get; }

    public EventScheduledBusMessageResponse(EventScheduledBusMessageData data)
    {
        Data = data;
        Id = Guid.NewGuid();
    }
}
