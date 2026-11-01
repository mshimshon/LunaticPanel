using System;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;

namespace LunaticPanel.Engine.Web.Hooks.Queries;

[QueryBusKey(PanelKeys.Queries.GetPanelVersion, ServiceLifetime = Core.Abstraction.Messaging.Common.EBusLifetime.Scoped)]
internal class GetPanelVersionQuery : IQueryBusHandler
{
    public Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry)
    {
        var result = typeof(IPlugin).Assembly.GetName().Version;
        Console.WriteLine($"Checking Version of the Running Panel ({result})");
        return qry.ReplyWith(result!.ToString(3));
    }
}
