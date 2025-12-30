using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EventBus;

public sealed class EventBusMessage : IEventBusMessage
{
    private readonly MessageKey _messageKey;

    public BusMessageData? Data { get; }
    public Guid Id { get; }
    public EventBusMessage(MessageKey messageKey, object? data = default)
    {
        Id = Guid.NewGuid();

        if (data != default)
            Data = new BusMessageData(data);
        _messageKey = messageKey;
    }

    public string GetId() => $"{_messageKey}";

    public BusMessageData? GetData() => Data;
    public Guid GetMessageId() => Id;
}
