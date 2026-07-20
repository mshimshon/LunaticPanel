using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;

namespace LunaticPanel.Core.Utils.FileWatcher;

internal sealed record FileTrackerKey
{

    public string FilePath { get; }
    public FileWatchEvent FileWatchEvents { get; }

    public FileTrackerKey(string filePath, FileWatchEvent fileWatchEvents)
    {
        FilePath = filePath;
        FileWatchEvents = fileWatchEvents;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FilePath, FileWatchEvents);
    }
}

