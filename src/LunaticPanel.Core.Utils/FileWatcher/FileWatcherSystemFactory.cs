using LunaticPanel.Core.Utils.Abstraction.FileWatcher;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;

namespace LunaticPanel.Core.Utils.FileWatcher;

internal sealed class FileWatcherSystemFactory : IFileWatcherSystemFactory
{
    private readonly IServiceProvider _serviceProvider;
    private List<IFileWatcherSystem> _watchRegistry = new();
    private bool _disposed;

    public FileWatcherSystemFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void CreateFileWatchUsing<TAction>(string path, string filePattern, FileWatchEvent[] whatToWatch, Func<TAction, IServiceProvider, Task> onNotify)
        where TAction : IFileWatcherAction
    {
        var watcher = new FileWatcherSystem<TAction>(path, filePattern, whatToWatch, onNotify, _serviceProvider);
        _watchRegistry.Add(watcher);
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            foreach (var item in _watchRegistry)
                item.Dispose();
        }
        _disposed = true;
    }

}
