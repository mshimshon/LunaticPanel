using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public interface IEventScheduledBus
{
    IReadOnlyCollection<string> GetAvailableKeys();
    public Task<EventScheduledBusMessageResponse> PublishAsync(IEventScheduledBusMessage msg, CancellationToken cancellationToken = default);
    bool HasKeyFor(MessageKey messageKey);
    bool HasKeyFor(string key);

}
