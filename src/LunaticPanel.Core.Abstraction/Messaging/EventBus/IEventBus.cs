using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public interface IEventBus
{
    IReadOnlyCollection<string> GetAvailableKeys();
    Task PublishAsync(IEventBusMessage evt, CancellationToken cancellationToken = default);
    bool HasKeyFor(MessageKey messageKey);
    bool HasKeyFor(string key);

}
