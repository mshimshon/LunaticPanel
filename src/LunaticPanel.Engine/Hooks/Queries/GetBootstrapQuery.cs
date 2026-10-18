using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Web.Boostrap;

namespace LunaticPanel.Engine.Web.Hooks.Queries;

[QueryBusKey(PluginKeys.Queries.FetchBootstrap, ServiceLifetime = Core.Abstraction.Messaging.Common.EBusLifetime.Scoped)]
internal class GetBootstrapQuery : IQueryBusHandler
{
    public async Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        return await qry.ReplyWith(Bootstrap.Configuration.KnownPlugins);
    }
}
