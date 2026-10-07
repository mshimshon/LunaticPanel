using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.Core.Messaging.EventBus;

public sealed class EventBus : IEventBus
{

    private readonly IEventBusExchange _eventBusExchange;
    private readonly IPluginInfo? _pluginInfo;
    private readonly ICrazyReport<EventBus> _crazyReport;

    public EventBus(IEventBusExchange eventBusExchange, IPluginInfo? pluginInfo, ICrazyReport<EventBus> crazyReport)
    {
        _eventBusExchange = eventBusExchange;
        _pluginInfo = pluginInfo;
        _crazyReport = crazyReport;
    }



    public Task PublishAsync(IEventBusMessage evt, CancellationToken cancellationToken = default)
    {
        if (_pluginInfo != default && evt.isTargetInternalId(_pluginInfo.PluginId))
            if (!_pluginInfo.IsTargetInternalKeyRegistered(evt.GetKey()))
            {
                var ex = new BusKeyNotRegisteredException(evt.GetKey(), _pluginInfo.PluginId);
                _crazyReport.ReportErrorException(ex.Message, ex);
                throw ex;
            }
        return _eventBusExchange.ExchangeAsync(evt, cancellationToken);
    }


    public IReadOnlyCollection<string> GetAvailableKeys() => _eventBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _eventBusExchange.AnyListenerFor(key);


}
