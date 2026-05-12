using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public interface IEventBusMessage : IBusMessage
{
    /// <summary>
    /// Gets the current tick count as a 64-bit integer value. (Unique)
    /// </summary>
    long GetTick();
    long SetTick(long current);

    /// <summary>
    /// Determines whether the ticker feature is enabled. (Default = False)
    /// </summary>
    bool HasTickerEnabled();

    IReadOnlyList<Guid>? GetTargets();
}
