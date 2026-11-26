using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class DownloadPanelUpdatePackagesHandler : IRequestHandler<DownloadPanelUpdatePackagesCommand>
{
    private readonly IPackageDownloader _packageDownloader;
    private readonly IEventScheduledBus _eventScheduledBus;
    private readonly IMediator _mediator;
    private readonly ICrazyReport<DownloadPanelUpdatePackagesHandler> _crazyReport;

    public DownloadPanelUpdatePackagesHandler(
        IPackageDownloader packageDownloader,
        IEventScheduledBus eventScheduledBus,
        IMediator mediator,
        ICrazyReport<DownloadPanelUpdatePackagesHandler> crazyReport)
    {
        _packageDownloader = packageDownloader;
        _eventScheduledBus = eventScheduledBus;
        _mediator = mediator;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    private async Task DownloadWithRetry(int count, PackageManifestPayload data, CancellationToken ct = default)
    {
        try
        {
            await _packageDownloader.DownloadAsync(data, ct);
            return;
        }
        catch (System.Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            if (count >= 3)
                return;
            await DownloadWithRetry(count, data, ct);
        }
    }
    public async Task HandleAsync(DownloadPanelUpdatePackagesCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");

            var foundUpgrades = await _mediator.ExecuteAsync(new GetPanelUpdateCompatiblePluginsQuery(request.MissingPackages, request.PanelVersion), ct);
            foreach (var item in foundUpgrades)
                await DownloadWithRetry(0, item, ct);
            await _eventScheduledBus.PublishAsync(EventScheduledBusMessage.Create(PanelKeys.Events.ScheduledEvents.AUTO_UPDATE_CYCLE), ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
    }

}
