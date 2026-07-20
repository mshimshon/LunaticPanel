using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;

namespace LunaticPanel.Core.Utils.Abstraction.FileWatcher;

public interface IFileWatcherSystemFactory : IDisposable
{
    void CreateFileWatchUsing<TAction>(string path, string filePattern, FileWatchEvent[] whatToWatch, Func<TAction, IServiceProvider, Task> onNotify)
        where TAction : IFileWatcherAction;
}
