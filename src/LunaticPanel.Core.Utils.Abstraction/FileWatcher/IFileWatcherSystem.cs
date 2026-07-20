namespace LunaticPanel.Core.Utils.Abstraction.FileWatcher;

public interface IFileWatcherSystem : IDisposable
{
    string Directory { get; init; }
    string FilePattern { get; init; }
}
public interface IFileWatcherSystem<TAction> : IFileWatcherSystem where TAction : IFileWatcherAction
{
}
