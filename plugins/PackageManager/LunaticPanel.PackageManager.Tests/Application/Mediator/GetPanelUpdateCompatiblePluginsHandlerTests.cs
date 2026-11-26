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
namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class GetPanelUpdateCompatiblePluginsHandlerTests
{
    private readonly IPanelControl _panelControl;
    private readonly IMediator _mediator;
    private readonly IPackageDownloader _packageDownloader;
    private readonly ICrazyReport<GetPanelUpdateCompatiblePluginsHandler> _crazyReport;


    private readonly GetPanelUpdateCompatiblePluginsHandler _handler;
    public GetPanelUpdateCompatiblePluginsHandlerTests()
    {
        _panelControl = Substitute.For<IPanelControl>();
        _mediator = Substitute.For<IMediator>();
        _packageDownloader = Substitute.For<IPackageDownloader>();
        _crazyReport = Substitute.For<ICrazyReport<GetPanelUpdateCompatiblePluginsHandler>>();
        _handler = new GetPanelUpdateCompatiblePluginsHandler(_packageDownloader, _crazyReport);
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
        _packageDownloader.GetLatestVersionAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<NuGetVersion>(), Arg.Any<CancellationToken>()).Returns(_updateFound);
        var result = await _handler.HandleAsync(new GetPanelUpdateCompatiblePluginsQuery(_updateFound.Select(p => p.Id).ToList(), NuGetVersion.Parse("1.0.0-beta.1")), TestContext.Current.CancellationToken);
        Assert.Equivalent(_updateFound, result);
    }


    [Fact]
    public async Task GetPanelUpdateCompatiblePluginsQuery_RethowHostUnkownException()
    {
        _packageDownloader.GetLatestVersionAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<NuGetVersion>(), Arg.Any<CancellationToken>()).Throws<Exception>();
        Func<Task> handler = () => _handler.HandleAsync(new GetPanelUpdateCompatiblePluginsQuery(new List<string>(), NuGetVersion.Parse("1.0.0-beta.1")), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(handler);
    }

}
