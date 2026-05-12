using CoreMap;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Engine.Application.Plugin.API.Dto;
using LunaticPanel.Engine.Keys.System;

namespace LunaticPanel.Engine.Application.Plugin.API.Queries.Handlers;

[QueryBusId(PluginKeys.Queries.FetchAll)]
internal class FetchPluginsHandler : IQueryBusHandler
{
    private readonly IPluginRegistry _pluginRegistry;
    private readonly ICoreMap _coreMap;

    public FetchPluginsHandler(IPluginRegistry pluginRegistry, ICoreMap coreMap)
    {
        _pluginRegistry = pluginRegistry;
        _coreMap = coreMap;
    }

    public Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        var resultDataUncleaned = _pluginRegistry.GetAll();
        var resultDataCleaned = resultDataUncleaned.Select(p => _coreMap.Map(p).To<PluginInfoResponse>()).ToArray();
        var busData = new BusMessageData(resultDataCleaned);
        var result = new QueryBusMessageResponse(busData);
        return Task.FromResult(result);
    }
}
