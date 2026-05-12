using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public sealed class EngineBusMessage : IEngineBusMessage
{
    private readonly BusMessageData? _data;
    private readonly Guid _messageId;
    private Guid? _circuitId;
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
    public Guid? GetOriginCircuitId() => _circuitId;
    public void SetOriginCircuitId(Guid id)
    {
        if (_circuitId == default) _circuitId = id;
    }

    //public Task<RenderFragment> GetRender() => Task.FromResult(_renderFragment);
}
