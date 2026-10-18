using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Web.Services;

namespace LunaticPanel.Engine.Web.Hooks.Queries;

[QueryBusKey(PluginKeys.Queries.FetchBootstrapFile, ServiceLifetime = Core.Abstraction.Messaging.Common.EBusLifetime.Scoped)]
internal class GetBootstrapFileQuery : IQueryBusHandler
{
    private readonly IBootstrapFileController _bootstrapFileController;

    public GetBootstrapFileQuery(IBootstrapFileController bootstrapFileController)
    {
        _bootstrapFileController = bootstrapFileController;
    }
    public async Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        Console.WriteLine("Loading Bootstrap File");
        var result = await _bootstrapFileController.LoadFile();
        return await qry.ReplyWith(result.KnownPlugins);
    }
}
