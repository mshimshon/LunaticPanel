using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EngineBus;

public sealed class EngineBusMessage : IEngineBusMessage
{
    private readonly BusMessageData? _data;
    private readonly Guid _messageId;
    private readonly MessageKey _messageKey;

    //private readonly RenderFragment _renderFragment;

    public EngineBusMessage(MessageKey messageKey)
    {
        _messageId = Guid.NewGuid();
        _messageKey = messageKey;
    }

    public EngineBusMessage(MessageKey messageKey, object data) : this(messageKey)
    {
        _data = new(data);
    }

    public string GetId() => _messageKey.ToString();

    public BusMessageData? GetData() => _data;
    public Guid GetMessageId() => _messageId;

    //public Task<RenderFragment> GetRender() => Task.FromResult(_renderFragment);
}
