using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Infrastructure.Interfaces;
using LunaticPanel.Engine.Keys.System;

namespace LunaticPanel.Engine.Hooks.Queries;

[QueryBusKey(PluginKeys.Queries.FETCH_BOOTSTRAP_FILE, ServiceLifetime = Core.Abstraction.Messaging.Common.EBusLifetime.Scoped)]
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
        return await qry.ReplyWith(result);
    }
}
