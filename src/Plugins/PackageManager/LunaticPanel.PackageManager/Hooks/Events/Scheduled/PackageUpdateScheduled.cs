using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Hooks.Events.Scheduled;

[EventScheduledBusKey(PackageManagerKeys.Event.Scheduled.PackageUpdateSchedule, 0, 30, RunAtStartup = true)]
internal class PackageUpdateScheduled : IEventScheduledBusHandler
{
    private readonly IStateAccessor<PackageUpdateScheduleState> _packageUpdateScheduleStateAccess;
    private readonly IDispatcher _dispatcher;
    private readonly ICrazyReport<PackageUpdateScheduled> _crazyReport;
    private static bool isExecuting = false;
    private static readonly object _lock = new object();
    public PackageUpdateScheduled(IStateAccessor<PackageUpdateScheduleState> packageUpdateScheduleStateAccess,
        IDispatcher dispatcher, ICrazyReport<PackageUpdateScheduled> crazyReport)
    {
        _packageUpdateScheduleStateAccess = packageUpdateScheduleStateAccess;
        _dispatcher = dispatcher;
        _crazyReport = crazyReport;
        _crazyReport.SetModule("Scheduler");
    }
    public EventScheduledBusMessageData DueToExecute(IEventScheduledBusMessage msg, CancellationToken ct = default)
    {
        var result = msg.ReplyWithAction(Exec);

        if (_packageUpdateScheduleStateAccess.State.CurrentlyUpdating != default)
            result.SkipExecution().NextTiming(0, 0, _packageUpdateScheduleStateAccess.State.Configuration.UpdateRunnerActiveFrequencySeconds);
        else if (_packageUpdateScheduleStateAccess.State.ToUpdate.Count() > 0)
            result.NextTiming(0, 0, _packageUpdateScheduleStateAccess.State.Configuration.UpdateRunnerActiveFrequencySeconds);
        else
            result.SkipExecution().NextTiming(0, 0, _packageUpdateScheduleStateAccess.State.Configuration.UpdateRunnerInactiveFrequencySeconds);
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
            if (_packageUpdateScheduleStateAccess.State.ToUpdate.Count() <= 0) return;
            var updatePkg = _packageUpdateScheduleStateAccess.State.ToUpdate.First();
            bool isCancelled = _packageUpdateScheduleStateAccess.State.CancelledRequests
                .Any(p => p.Info.PackageId == updatePkg.Info.PackageId);
            if (isCancelled)
                await _dispatcher.Prepare<InstallNextUpdateDoneAction>()
                    .With(p => p.ToRemove, updatePkg)
                    .Await()
                    .DispatchAsync(ct);
            else
                await _dispatcher.Prepare<InstallNextUpdateAction>()
                    .With(p => p.Package, updatePkg)
                    .Await()
                    .DispatchAsync(ct);
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
