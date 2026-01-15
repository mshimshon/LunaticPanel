using System.Text.Json;

namespace LunaticPanel.Core.Abstraction.StateManagement;

public sealed record BusStatePayload
{
    public string Action { get; }
    public string State { get; }
    public DateTime WrittenAt { get; } = DateTime.UtcNow;

    public BusStatePayload(object state, object action)
    {
        State = JsonSerializer.Serialize(state);
        Action = JsonSerializer.Serialize(action);
    }
    public T GetAction<T>() => JsonSerializer.Deserialize<T>(Action)!;
    public T GetState<T>() => JsonSerializer.Deserialize<T>(State)!;
}

