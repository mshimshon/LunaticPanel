using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using StatePulse.Net;
namespace LunaticPanel.PackageManager.Hooks.Events.Scheduled;

[EventScheduledBusKey(LPPackageManagerKeys.Event.Scheduled.PackageUpdateSchedule, 0, 30, RunAtStartup = true)]
internal class PackageUpdateScheduled : IEventScheduledBusHandler
{
    private readonly IStateAccessor<PackageUpdateScheduleState> _packageUpdateScheduleStateAccess;
    private readonly IDispatcher _dispatcher;
    private readonly ICrazyReport<PackageUpdateScheduled> _crazyReport;
    private readonly IStateAccessor<PackageUpdateState> _packageUpdateState;
    private readonly IHostControl _hostControl;
    private static bool isExecuting = false;
    private static readonly object _lock = new object();
    public PackageUpdateScheduled(IStateAccessor<PackageUpdateScheduleState> packageUpdateScheduleStateAccess,
        IDispatcher dispatcher, ICrazyReport<PackageUpdateScheduled> crazyReport,
        IStateAccessor<PackageUpdateState> packageUpdateState, IHostControl hostControl)
    {
        // PackageUpdateState
        _packageUpdateScheduleStateAccess = packageUpdateScheduleStateAccess;
        _dispatcher = dispatcher;
        _crazyReport = crazyReport;
        _packageUpdateState = packageUpdateState;
        _hostControl = hostControl;
        _crazyReport.SetModule("Scheduler");
    }
    public EventScheduledBusMessageData DueToExecute(IEventScheduledBusMessage msg, CancellationToken ct = default)
    {
        var result = msg.ReplyWithAction(Exec);
        if (_packageUpdateState.State.IsLoading)
            result.SkipExecution().NextTiming(0, 0, _packageUpdateScheduleStateAccess.State.Configuration.UpdateRunnerActiveFrequencySeconds);
        else
            result.NextTiming(0, 0, _packageUpdateScheduleStateAccess.State.Configuration.UpdateRunnerInactiveFrequencySeconds);
        return result;
    }

    public async Task Exec(CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (isExecuting) return;
            isExecuting = true;
        }

        try
        {
            await _dispatcher.Prepare<PeriodicCheckPackageUpdatesAction>().Await().DispatchAsync();
            var updates = _packageUpdateState.State.FoundUpdates.ToList();
            _crazyReport.Report("Package Update Found {0}", updates.Count);
            if (updates.Count <= 0) return;
            await _dispatcher.Prepare<AutoUpdatePackagesAction>().Await().DispatchAsync(ct);
            // Auto Reload
            if (_packageUpdateScheduleStateAccess.State.Configuration.AutoRestart)
                await _hostControl.RestartAsync();
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
        }
        finally
        {
            lock (_lock)
            {
                isExecuting = false;
            }
        }

    }
}
