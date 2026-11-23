using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Bootstraper;
using LunaticPanel.Engine.Hooks.Payloads.Responses;
using LunaticPanel.Engine.Infrastructure.Interfaces;


namespace LunaticPanel.Engine.Hooks.Queries;

[QueryBusKey(PluginKeys.Queries.FETCH_ALL)]
internal class FetchPluginsQuery : IQueryBusHandler
{
    private readonly IPluginRegistry _pluginRegistry;

    public FetchPluginsQuery(IPluginRegistry pluginRegistry)
    {
        _pluginRegistry = pluginRegistry;
    }

    public Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        var currentlyLoadedPlugins = _pluginRegistry.ActivePlugins;
        var allKnownPlugins = _pluginRegistry.KnownPlugins;
        var failedPlugins = _pluginRegistry.FailedPlugins;

        List<PluginInfoResponse> pluginInfoResult = new();
        foreach (var item in allKnownPlugins)
        {

            bool isFailed = failedPlugins.ContainsKey(item.Manifest.Id);
            bool hasLoaded = currentlyLoadedPlugins.Any(p => p.Manifest.Id.Equals(item.Id));
            var response = new PluginInfoResponse(item.Id, item.Version, item.PanelVersion, item.StartupState)
            {
                Failure = !isFailed ? default : failedPlugins[item.Manifest.Id],
                Loaded = hasLoaded
            };
            pluginInfoResult.Add(response);
        }

        var busData = new BusMessageData(pluginInfoResult);
        var result = new QueryBusMessageResponse(busData);
        return Task.FromResult(result);
    }
}
