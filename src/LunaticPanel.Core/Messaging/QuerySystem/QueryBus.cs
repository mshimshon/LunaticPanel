using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.Core.Messaging.QuerySystem;

public sealed class QueryBus : IQueryBus
{

    private readonly IQueryBusExchange _queryBusExchange;
    private readonly IPluginInfo? _pluginInfo;
    private readonly ICrazyReport<QueryBus> _crazyReport;

    public QueryBus(IQueryBusExchange queryBusExchange, IPluginInfo? pluginInfo, ICrazyReport<QueryBus> crazyReport)
    {
        _queryBusExchange = queryBusExchange;
        _pluginInfo = pluginInfo;
        _crazyReport = crazyReport;
    }


    public IReadOnlyCollection<string> GetAvailableKeys() => _queryBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _queryBusExchange.AnyListenerFor(key);


    public Task<QueryBusMessageResponse> QueryAsync(IQueryBusMessage qry, CancellationToken cancellationToken = default)
    {
        if (_pluginInfo != default && qry.isTargetInternalId(_pluginInfo.PluginId))
            if (!_pluginInfo.IsTargetInternalKeyRegistered(qry.GetKey()))
            {
                var ex = new BusKeyNotRegisteredException(qry.GetKey(), _pluginInfo.PluginId);
                _crazyReport.ReportErrorException(ex.Message, ex);
                throw ex;
            }
        return _queryBusExchange.ExchangeAsync(qry, cancellationToken);
    }

}
