namespace LunaticPanel.Core.Abstraction.Messaging.EventBus;

public enum EventBusSpreadType
{
    /// <summary>
    /// (Default) Will not trigger same event listener across circuits.
    /// </summary>
    SelfContained = 0,
    /// <summary>
    /// Spread across all blazor circuit for this listenner.
    /// </summary>
    CrossCircuitAll = 1,
    /// <summary>
    /// Spread Across all circuits except the sender's circuit.
    /// </summary>
    CrossCircuitExcludeSender = 2

}
