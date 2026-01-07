namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public interface IEngineBusReceiver
{
    Task<EngineBusResponse[]> IncomingMessageAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default);
    IReadOnlyCollection<string> WhatDoYouListenFor();
    bool DoYouListenTo(string key);
}
