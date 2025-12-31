using CoreMap;
using LunaticPanel.Core.Messaging.Common;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application.Plugin.API.Dto;
using LunaticPanel.Engine.Keys.System;

namespace LunaticPanel.Engine.Application.Plugin.API.Queries.Handlers;

[QueryBusId(PluginKeys.fetchAll)]
internal class FetchPluginsHandle : IQueryBusHandler
{
    private readonly IPluginRegistry _pluginRegistry;
    private readonly ICoreMap _coreMap;

    public FetchPluginsHandle(IPluginRegistry pluginRegistry, ICoreMap coreMap)
    {
        _pluginRegistry = pluginRegistry;
        _coreMap = coreMap;
    }

    public Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        var resultDataUncleaned = _pluginRegistry.GetAll();
        var resultDataCleaned = resultDataUncleaned.Select(p => _coreMap.Map(p).To<PluginInfoDto>()).ToArray();
        var busData = new BusMessageData(resultDataCleaned);
        var result = new QueryBusMessageResponse(busData);
        return Task.FromResult(result);
    }
}
