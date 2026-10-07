namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public interface IQueryBusReceiver
{
    Task<QueryBusMessageResponse?> IncomingMessageAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default);
    IReadOnlyCollection<string> WhatDoYouListenFor();
    bool DoYouListenTo(string key);
}
