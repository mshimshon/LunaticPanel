using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;

namespace LunaticPanel.Core.Utils.Abstraction.FileWatcher;

public interface IFileWatcherAction
{
    DateTime Date { get; set; }
    FileWatchEvent Event { get; set; }
    string? FullName { get; set; }
    string? FileName { get; set; }
}
