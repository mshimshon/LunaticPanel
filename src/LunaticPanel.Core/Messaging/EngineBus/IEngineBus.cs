using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EngineBus;

public interface IEngineBus
{
    Task<EngineBusResponse[]> ExecAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default);
    IReadOnlyCollection<string> GetAvailableKeys();
    bool HasKeyFor(MessageKey messageKey);
    bool HasKeyFor(string key);


}
