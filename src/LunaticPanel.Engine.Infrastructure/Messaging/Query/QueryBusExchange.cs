using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Query;

internal class QueryBusExchange : IQueryBusExchange
{
    private readonly ICircuitRegistry _circuitRegistry;
    private readonly IQueryBusReceiver _queryBusReceiver;

    private Guid CircuitId => _circuitRegistry.CurrentCircuit.CircuitId;
    public QueryBusExchange(ICircuitRegistry circuitRegistry, IQueryBusReceiver queryBusReceiver)
    {
        _circuitRegistry = circuitRegistry;
        _queryBusReceiver = queryBusReceiver;
    }
    public bool AnyListenerFor(string key)
    {
        key = key.ToLower();
        var result = _queryBusReceiver.DoYouListenTo(key);
        if (result) return result;
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;

            var pluginBusReceiver = item.Value.GetRequired<IQueryBusReceiver>();
            result = pluginBusReceiver.DoYouListenTo(key);
            if (result) return result;
        }
        return false;
    }

    public async Task<QueryBusMessageResponse> ExchangeAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default)
    {
        qry.SetOriginCircuitId(CircuitId);
        var message = qry;

        var resultFromHost = await _queryBusReceiver.IncomingMessageAsync(message, cancellationToken);
        if (resultFromHost != default) return resultFromHost;

        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (qry.TargetCircuit != Guid.Empty && item.Key.CircuitId != qry.TargetCircuit)
                continue;
            if (qry.TargetCircuit == Guid.Empty && item.Key.CircuitId != CircuitId)
                continue;
            var pluginBusReceiver = item.Value.GetRequired<IQueryBusReceiver>();
            var resultFromHandler = await pluginBusReceiver.IncomingMessageAsync(message, cancellationToken);
            if (resultFromHandler != default) return resultFromHandler;
        }
        return new()
        {
            Error = new("INTERNAL", $"No Queries Handler from for {qry.GetId()}"),
            Origin = nameof(QueryBusExchange)
        };
    }

    public IReadOnlyCollection<string> GetAvailableKeys()
    {
        var hostListeningList = _queryBusReceiver.WhatDoYouListenFor();
        List<string> result = new();
        result.AddRange(hostListeningList);
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;
            var pluginBusReceiver = item.Value.GetRequired<IQueryBusReceiver>();
            var handlerResults = pluginBusReceiver.WhatDoYouListenFor();
            result.AddRange(handlerResults.Where(p => !result.Contains(p)));
        }
        return result.AsReadOnly();
    }
}
