using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Core.Messaging.QuerySystem.Exceptions;
using LunaticPanel.Engine.Application.Messaging.Query;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Infrastructure.Messaging.Query;

internal class QueryBus : IQueryBus
{

    private readonly IServiceProvider _serviceProvider;
    private readonly IQueryBusRegistry _queryBusRegistry;

    public QueryBus(IServiceProvider serviceProvider, IQueryBusRegistry queryBusRegistry)
    {
        _serviceProvider = serviceProvider;
        _queryBusRegistry = queryBusRegistry;
    }

    private static Task<QueryBusMessageResponse> ExecuteHandler(IQueryBusMessage qry, IServiceProvider serviceProvider, Type handlerType)
    {
        var handler = serviceProvider.GetRequiredService(handlerType) as IQueryBusHandler;
        return handler!.HandleAsync(qry);
    }

    public IReadOnlyCollection<string> GetAllQueryIds() => _queryBusRegistry.GetAllAvailableIds();
    public async Task<QueryBusMessageResponse> QueryAsync(IQueryBusMessage qry)
    {
        string id = qry.GetId();
        var registry = _queryBusRegistry;
        try
        {
            var handler = registry.GetRegistryFor(id);
            try
            {
                var result = await ExecuteHandler(qry, _serviceProvider, handler);
                return result with { Origin = handler.FullName! };
            }
            catch (QueryBusMessageException ex)
            {
                // TODO: OPEN TELEMETRY? OR CAP EVENT
                return new() { Error = ex, Origin = handler.FullName! };
            }
            catch (Exception ex)
            {
                return new() { Error = new("INTERNAL", ex.Message), Origin = handler.FullName! };
            }


        }
        catch (Exception ex)
        {
            return new() { Error = new("INTERNAL", ex.Message), Origin = id };
        }


    }
}
