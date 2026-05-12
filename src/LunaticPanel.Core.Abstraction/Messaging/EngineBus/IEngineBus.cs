using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public interface IEngineBus
{
    Task<EngineBusResponse[]> ExecAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default);
    IReadOnlyCollection<string> GetAvailableKeys();
    bool HasKeyFor(MessageKey messageKey);
    bool HasKeyFor(string key);


}
