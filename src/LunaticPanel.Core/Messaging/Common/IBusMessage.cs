namespace LunaticPanel.Core.Messaging.Common;

public interface IBusMessage
{
    string GetId();
    BusMessageData? GetData();
    Guid GetMessageId();
}
