using LunaticPanel.Core.Utils.Abstraction.FileWatcher;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using System.Collections.Concurrent;

namespace LunaticPanel.Core.Utils.FileWatcher;

internal class FileWatcherSystem<TAction> : IFileWatcherSystem<TAction> where TAction : IFileWatcherAction
{
    private bool _disposed;
    private readonly FileSystemWatcher _fileSystemWatcher;
    private readonly ICrazyReport? _crazyReport;
    private readonly IServiceProvider _serviceProvider;
    private readonly FileWatchEvent[] _whatToWatch;
    private readonly Func<TAction, IServiceProvider, Task> _onNotify;

    public string Directory { get; init; }
    public string FilePattern { get; init; }

    private readonly string _path;
    private readonly ConcurrentQueue<FileNotifyQueueElement> _changeQueue = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _queueWorker;

    private readonly ConcurrentDictionary<FileTrackerKey, long> _pendingByPath = new();
    public FileWatcherSystem(string path, string filePattern, FileWatchEvent[] whatToWatch, Func<TAction, IServiceProvider, Task> onNotify, IServiceProvider serviceProvider)
    {

        Directory = path;
        FilePattern = filePattern;
        _crazyReport = (ICrazyReport?)serviceProvider.GetService(typeof(ICrazyReport<TAction>));
        _serviceProvider = serviceProvider;
        _crazyReport?.SetModule("StateFileWatcher");
        _whatToWatch = whatToWatch;
        _onNotify = onNotify;
        var linuxWorkAroundFileName = Path.Combine(path, Path.GetRandomFileName());
        _crazyReport?.ReportInfo("Creating TMP Fix Workaround: {0}", linuxWorkAroundFileName);
        File.WriteAllText(linuxWorkAroundFileName, "");
        _fileSystemWatcher = new FileSystemWatcher(path, filePattern);
        _fileSystemWatcher!.EnableRaisingEvents = true;
        File.Delete(linuxWorkAroundFileName);

        _fileSystemWatcher.Created += DispatchNotify;
        _fileSystemWatcher.Changed += DispatchNotify;
        _fileSystemWatcher.Deleted += DispatchNotify;
        _fileSystemWatcher.Renamed += DispatchNotify;
        _path = Path.Combine(path, filePattern);
        _queueWorker = Task.Run(() => WatchQueue(_cts.Token));
        _crazyReport?.ReportInfo("Watching file {0} with following changes: {1}", _path, string.Join(", ", whatToWatch));
    }


    private void DispatchNotify(object _, FileSystemEventArgs e)
    {

        _crazyReport?.ReportInfo("Attempt to Enqueue ({1}) for {0}", _path, e.ChangeType);
        FileWatchEvent eventTypeToWatch = e.ChangeType switch
        {
            WatcherChangeTypes.Created => FileWatchEvent.Created,
            WatcherChangeTypes.Deleted => FileWatchEvent.Removed,
            WatcherChangeTypes.Changed => FileWatchEvent.Updated,
            WatcherChangeTypes.Renamed => FileWatchEvent.Renamed,
            _ => FileWatchEvent.Any
        };

        FileWatchEvent eventType = eventTypeToWatch;
        bool isWatchingAnything = _whatToWatch.Contains(FileWatchEvent.Any);
        bool isWatchingTheEvent = isWatchingAnything || eventType == FileWatchEvent.Any ? isWatchingAnything : _whatToWatch.Contains(eventType);
        if (!isWatchingTheEvent)
        {
            _crazyReport?.ReportWarning("isWatchingAnything {0};  isWatchingTheEvent {1};", isWatchingAnything, isWatchingTheEvent);
            _crazyReport?.ReportWarning("Ignored {0} for {1} ", eventType, _path);
            return;
        }
        var version = _pendingByPath.AddOrUpdate(
            new(e.FullPath, eventType),
            _ => 1,
            (_, v) => v + 1
        );

        var action = new FileNotifyQueueElement()
        {
            Version = version,
            EventType = eventType,
            Path = e.FullPath,
            Action = () => Notify(e, eventType)
        };
        _crazyReport?.Report("Enqueue {0}", action.ToString());
        Enqueue(action);


    }
    private async Task Notify(FileSystemEventArgs args, FileWatchEvent eventType)
    {

        var action = Activator.CreateInstance<TAction>();
        if (action == null) return;
        action.Event = eventType;
        action.FullName = args.FullPath;
        action.Date = DateTime.UtcNow;
        action.FileName = args.Name!;
        try
        {
            await _onNotify.Invoke(action, _serviceProvider);
        }
        catch (Exception ex)
        {
            _crazyReport?.ReportError("Error Running {0} with Exception {1}", action.GetType(), ex.Message);
        }

    }
    private readonly SemaphoreSlim _signal = new(0);

    private void Enqueue(FileNotifyQueueElement work)
    {
        _changeQueue.Enqueue(work);
        _signal.Release();
    }

    private async Task WatchQueue(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await _signal.WaitAsync(ct).ConfigureAwait(false);

            if (_changeQueue.TryDequeue(out var nextAction))
            {
                var key = new FileTrackerKey(nextAction.Path, nextAction.EventType);
                try
                {
                    // only keep lastest update and kill any stale
                    //_pendingByPath.TryGetValue(next.Path, )
                    _pendingByPath.TryGetValue(key, out long currentVersion);
                    bool isStaleAction = currentVersion == default || currentVersion > nextAction.Version;
                    _crazyReport?.Report("Is Queue Action Stale? {0}", isStaleAction);
                    if (isStaleAction) continue;
                    await nextAction.Action().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _crazyReport?.ReportError("WatchQueue Error {0}", ex.Message);
                    // TODO: Handle that shit
                }
                finally
                {
                    _pendingByPath.TryGetValue(key, out var currentVersion);
                    if (currentVersion != default && currentVersion == nextAction.Version)
                    {
                        if (_pendingByPath.TryRemove(key, out _))
                            _crazyReport?.Report("WatchQueue Removing Version Tracking for {0} ({1} == {2})", nextAction.Path, currentVersion, nextAction.Version);
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _cts.Cancel();
            _fileSystemWatcher.Dispose();
        }

        // free unmanaged resources here

        _disposed = true;
    }


}
