using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Infrastructure.Interfaces;
using LunaticPanel.Engine.Keys.System;

namespace LunaticPanel.Engine.Hooks.Queries;

[QueryBusKey(PluginKeys.Queries.FETCH_BOOTSTRAP, ServiceLifetime = Core.Abstraction.Messaging.Common.EBusLifetime.Scoped)]
internal class GetBootstrapQuery : IQueryBusHandler
{
    private readonly IPluginRegistry _pluginRegistry;

    public GetBootstrapQuery(IPluginRegistry pluginRegistry)
    {
        _pluginRegistry = pluginRegistry;
    }
    public async Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        return await qry.ReplyWith(_pluginRegistry.KnownPlugins);
    }
}
