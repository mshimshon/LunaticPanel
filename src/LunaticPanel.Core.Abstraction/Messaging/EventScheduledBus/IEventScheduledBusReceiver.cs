namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public interface IEventScheduledBusReceiver
{
    Task<EventScheduledBusMessageResponse?> IncomingMessageAsync(IEventScheduledBusMessage msg, CancellationToken cancellationToken = default);
    IReadOnlyCollection<string> WhatDoYouListenFor();
    bool DoYouListenTo(string key);
}
