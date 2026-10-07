using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public interface IQueryBusMessage : IBusMessage
{
    Guid TargetCircuit { get; }
}
