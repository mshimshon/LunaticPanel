using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Tests.Application.Mock;
using StatePulse.Net;
using System;
using LunaticPanel.Core.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using LunaticPanel.PackageManager.Application.Payloads;
using NSubstitute;
using NSubstitute.Extensions;
using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute.ExceptionExtensions;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using LunaticPanel.PackageManager.Application.Services;
using NSubstitute.Core.Arguments;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class DownloadPanelUpdatePackagesHandlerTests
{
    private readonly IPanelControl _panelControl;
    private readonly IEventScheduledBus _eventScheduledBus;
    private readonly IMediator _mediator;
    private readonly IPackageDownloader _packageDownloader;
    private readonly ICrazyReport<DownloadPanelUpdatePackagesHandler> _crazyReport;


    private readonly DownloadPanelUpdatePackagesHandler _handler;
    public DownloadPanelUpdatePackagesHandlerTests()
    {
        _panelControl = Substitute.For<IPanelControl>();
        _mediator = Substitute.For<IMediator>();
        _packageDownloader = Substitute.For<IPackageDownloader>();
        _eventScheduledBus = Substitute.For<IEventScheduledBus>();
        _crazyReport = Substitute.For<ICrazyReport<DownloadPanelUpdatePackagesHandler>>();
        _handler = new DownloadPanelUpdatePackagesHandler(_packageDownloader, _eventScheduledBus, _mediator, _crazyReport);
    }

    [Fact]
    public async Task GetPanelUpdateCompatiblePluginsQuery_Pass()
    {
        IEnumerable<PackageManifestPayload> _updateFound = [
            PackageExt.CreatePackageManifest("panel.auth.azure", "Azure Authentication", "2.4.1-beta.3",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.monitoring.health", "Health Monitoring", "1.8.0-rc.1",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.backup.agent", "Backup Agent", "3.0.0-alpha.7",  "5.2.0"),
            PackageExt.CreatePackageManifest("panel.notifications.slack", "Slack Notifications", "4.2.5-preview.12",  "5.1.2"),
            PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "5.2.0-preview.1")
        ];
        _mediator.ExecuteAsync(Arg.Any<DownloadPanelUpdatePackagesCommand>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_updateFound));
        _packageDownloader.GetLatestVersionAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<NuGetVersion>(), Arg.Any<CancellationToken>()).Returns(_updateFound);
        var message = new DownloadPanelUpdatePackagesCommand(_updateFound.Select(p => p.Id).ToList(), NuGetVersion.Parse("1.0.0-beta.1"));
        await _handler.HandleAsync(message, TestContext.Current.CancellationToken);
        await _eventScheduledBus.Received().PublishAsync(Arg.Any<IEventScheduledBusMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPanelUpdateCompatiblePluginsQuery_RethowHostUnkownException()
    {
        _mediator.ExecuteAsync(Arg.Any<GetPanelUpdateCompatiblePluginsQuery>(), Arg.Any<CancellationToken>()).Throws<Exception>();
        var message = new DownloadPanelUpdatePackagesCommand(new List<string>(), NuGetVersion.Parse("1.0.0-beta.1"));
        Func<Task> handler = () => _handler.HandleAsync(message, TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(handler);
    }

}
