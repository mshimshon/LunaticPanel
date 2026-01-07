namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public interface IEventBusReceiver
{
    Task IncomingMessageAsync(IEventBusMessage eventBusMessage, CancellationToken cancellationToken = default);
    IReadOnlyCollection<string> WhatDoYouListenFor();
    bool DoYouListenTo(string key);
}
