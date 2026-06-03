using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.Core.Messaging.EngineBus;

public class EngineBus : IEngineBus
{
    private readonly IEngineBusExchange _engineBusExchange;
    private readonly IPluginInfo _pluginInfo;
    private readonly ICrazyReport<EngineBus> _crazyReport;

    public EngineBus(IEngineBusExchange engineBusExchange, IPluginInfo pluginInfo, ICrazyReport<EngineBus> crazyReport)
    {
        _engineBusExchange = engineBusExchange;
        _pluginInfo = pluginInfo;
        _crazyReport = crazyReport;
    }


    public Task<EngineBusResponse[]> ExecAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default)
    {
        if (engineBusRender.isTargetInternalId(_pluginInfo.PluginId))
            if (!_pluginInfo.IsTargetInternalKeyRegistered(engineBusRender.GetKey()))
            {
                var ex = new BusKeyNotRegisteredException(engineBusRender.GetKey(), _pluginInfo.PluginId);
                _crazyReport.ReportErrorException(ex.Message, ex);
                throw ex;
            }
        return _engineBusExchange.ExchangeAsync(engineBusRender, cancellationToken);
    }


    public IReadOnlyCollection<string> GetAvailableKeys() => _engineBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _engineBusExchange.AnyListenerFor(key);



}
