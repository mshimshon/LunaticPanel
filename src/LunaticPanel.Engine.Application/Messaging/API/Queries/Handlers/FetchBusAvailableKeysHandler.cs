using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application.Messaging.API.Dto;
using LunaticPanel.Engine.Application.Messaging.EngineBus;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Application.Messaging.Query;
using LunaticPanel.Engine.Core.System;

namespace LunaticPanel.Engine.Application.Messaging.API.Queries.Handlers;

[QueryBusId(BusKeys.Queries.FetchAvailableBuses)]
internal class FetchBusAvailableKeysHandler : IQueryBusHandler
{
    private readonly IEngineBusRegistry _engineBusRegistry;
    private readonly IEventBusRegistry _eventBusRegistry;
    private readonly IQueryBusRegistry _queryBusRegistry;

    public FetchBusAvailableKeysHandler(IEngineBusRegistry engineBusRegistry, IEventBusRegistry eventBusRegistry, IQueryBusRegistry queryBusRegistry)
    {
        _engineBusRegistry = engineBusRegistry;
        _eventBusRegistry = eventBusRegistry;
        _queryBusRegistry = queryBusRegistry;
    }
    public Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        var requestData = qry.GetData()?.GetDataAs<GetMessageBusAvailableKeysRequest>();
        var engineBusKeys = _engineBusRegistry.GetAllAvailableIds();
        var eventBusKeys = _eventBusRegistry.GetAllAvailableIds();
        var queryBusKeys = _queryBusRegistry.GetAllAvailableIds();
        List<BusInfoResponse> busInfos = new();
        bool includeAll =
            requestData == default ||
            (requestData.IncludeEngine && requestData.IncludeQuery && requestData.IncludeEvent) ||
            (!requestData.IncludeEngine && !requestData.IncludeQuery && !requestData.IncludeEvent);
        if (includeAll || requestData!.IncludeEngine)
            busInfos.AddRange(engineBusKeys.Select(p => new BusInfoResponse() { Key = p }));

        if (includeAll || requestData!.IncludeQuery)
            busInfos.AddRange(queryBusKeys.Select(p => new BusInfoResponse() { Key = p }));

        if (includeAll || requestData!.IncludeEvent)
            busInfos.AddRange(eventBusKeys.Select(p => new BusInfoResponse() { Key = p }));

        QueryBusMessageResponse result = new(new(busInfos));
        return Task.FromResult(result);
    }
}
