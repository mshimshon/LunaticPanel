
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Web.Services;

namespace LunaticPanel.Engine.Web.Hooks.Events.Scheduled;

[EventScheduledBusKey(PanelKeys.Events.ScheduledEvents.AutoUpdateCycle, 1, 0, 0, 0, 0, RunAtStartup = true)]
internal class PackageUpdateScheduled : IEventScheduledBusHandler
{
    private readonly ICrazyReport<PackageUpdateScheduled> _crazyReport;
    private readonly IPanelUpdateService _panelUpdateService;
    private readonly ICircuitRegistry _circuitRegistry;

    private static bool isExecuting = false;
    private static readonly Lock _lock = new();
    private TimeSpan _configuredTiming = new(1, 0, 0, 0);
    public PackageUpdateScheduled(ICrazyReport<PackageUpdateScheduled> crazyReport, IPanelUpdateService panelUpdateService, ICircuitRegistry circuitRegistry)
    {
        // PackageUpdateState
        _crazyReport = crazyReport;
        _panelUpdateService = panelUpdateService;
        _circuitRegistry = circuitRegistry;

        _crazyReport.SetModule("Scheduler");
        string? hoursCheck = Environment.GetEnvironmentVariable("LP_UPDATE_HOURS");
        string? minutesCheck = Environment.GetEnvironmentVariable("LP_UPDATE_MINUTES");
        if (int.TryParse(hoursCheck, out var hours))
            _configuredTiming = new(0, hours, 0, 0);
        else if (int.TryParse(minutesCheck, out var minutes))
            _configuredTiming = new(0, 0, minutes < 15 ? 15 : minutes, 0);
    }

    public EventScheduledBusMessageData DueToExecute(IEventScheduledBusMessage msg, CancellationToken ct = default)
    {
        var result = msg.ReplyWithAction(Exec);


        if (isExecuting)
            result.SkipExecution().NextTiming(0, 0, 30);
        else
            result.NextTiming(_configuredTiming);
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
            if (_panelUpdateService.BlockedByPlugin) return;
            if (_panelUpdateService.Staged != default) return;
            // Get the Latest Update
            await _panelUpdateService.DownloadUpdateAsync(ct);
            // Check if upgrade folder contains same plugins versions as installed/applied compatible with the upcoming upgrade.
            await _panelUpdateService.CheckPluginsAsync(ct);
            // if still missing plugins, skip do not install upgrade.
            if (_panelUpdateService.AwaitingForPluginCompatibilityUpdate)
                return;
            // All plugins are compatible with the next major panel upgrade, auto upgrade is ready to deploy.
            if (_panelUpdateService.Downloaded != default && _panelUpdateService.Install == default)
                await _panelUpdateService.InstallUpdateAsync(ct);

        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            // TODO: Event Failure of Upgrade (Critical and should be notifying sys admin.)
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
