using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.Core.Messaging.EventScheduledBus;

public sealed class EventScheduledBus : IEventScheduledBus
{

    private readonly IEventScheduledBusExchange _eventScheduledBusExchange;
    private readonly IPluginInfo _pluginInfo;
    private readonly ICrazyReport<EventScheduledBus> _crazyReport;

    public EventScheduledBus(IEventScheduledBusExchange eventBusExchange, IPluginInfo pluginInfo, ICrazyReport<EventScheduledBus> crazyReport)
    {
        _eventScheduledBusExchange = eventBusExchange;
        _pluginInfo = pluginInfo;
        _crazyReport = crazyReport;
    }


    public Task<EventScheduledBusMessageResponse> PublishAsync(IEventScheduledBusMessage evt, CancellationToken cancellationToken = default)
    {
        if (evt.isTargetInternalId(_pluginInfo.PluginId))
            if (!_pluginInfo.IsTargetInternalKeyRegistered(evt.GetKey()))
            {
                var ex = new BusKeyNotRegisteredException(evt.GetKey(), _pluginInfo.PluginId);
                _crazyReport.ReportErrorException(ex.Message, ex);
                throw ex;
            }
        return _eventScheduledBusExchange.ExchangeAsync(evt, cancellationToken);
    }

    public IReadOnlyCollection<string> GetAvailableKeys() => _eventScheduledBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _eventScheduledBusExchange.AnyListenerFor(key);


}
