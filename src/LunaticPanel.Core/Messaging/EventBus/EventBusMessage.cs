using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EventBus;

public sealed class EventBusMessage : IEventBusMessage
{

    public BusMessageData? Data { get; }
    public string EventBaseId { get; }
    public string EventType { get; }
    public Guid Id { get; }
    public EventBusMessage(string eventBaseId, string eventType, object? data = default)
    {
        Id = Guid.NewGuid();

        if (data != default)
            Data = new BusMessageData(data);
        EventBaseId = eventBaseId;
        EventType = eventType;
    }

    public string GetId() => $"{EventBaseId}.{EventType}";

    public BusMessageData? GetData() => Data;
    public Guid GetMessageId() => Id;
}
