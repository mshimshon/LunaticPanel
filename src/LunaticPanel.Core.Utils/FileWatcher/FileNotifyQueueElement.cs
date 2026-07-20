using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;

namespace LunaticPanel.Core.Utils.FileWatcher;

internal sealed record FileNotifyQueueElement
{
    public string Path { get; init; } = string.Empty;
    public long Version { get; init; }
    public FileWatchEvent EventType { get; init; }
    public Func<Task> Action { get; init; } = default!;
}

