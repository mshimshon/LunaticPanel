using LunaticPanel.Core.Utils.Abstraction.FileWatcher;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;

namespace LunaticPanel.Package.LocalServer.Infrastructure.Services.FileWatcher;

public sealed record WatchLocation : IFileWatcherAction
{
    public DateTime Date { get; set; }
    public FileWatchEvent Event { get; set; }
    public string? FullName { get; set; }
    public string? FileName { get; set; }
}
