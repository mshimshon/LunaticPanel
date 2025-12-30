using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Core.Messaging.QuerySystem.Exceptions;
using LunaticPanel.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Application.Messaging.Query;

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
                QueryBusMessageResponse result;
                if (handler.Plugin != default)
                {
                    var pluginType = handler.Plugin.GetType();
                    var serviceType = typeof(IPluginService<>).MakeGenericType(pluginType);
                    var pluginService = _serviceProvider.GetRequiredService(serviceType) as IPluginService;
                    result = await ExecuteHandler(qry, pluginService!, handler.HandlerType);
                }
                else
                    result = await ExecuteHandler(qry, _serviceProvider, handler.HandlerType);
                return result with { Origin = handler.HandlerType.FullName! };
            }
            catch (QueryBusMessageException ex)
            {
                // TODO: OPEN TELEMETRY? OR CAP EVENT
                return new() { Error = ex, Origin = handler.HandlerType.FullName! };
            }
            catch (Exception ex)
            {
                return new() { Error = new("INTERNAL", ex.Message), Origin = handler.HandlerType.FullName! };
            }


        }
        catch (Exception ex)
        {
            return new() { Error = new("INTERNAL", ex.Message), Origin = id };
        }


    }
}
