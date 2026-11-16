using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using MaksimShimshon.Mediator;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NuGet.Versioning;
using StatePulse.Net;
using System.Diagnostics.CodeAnalysis;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class PeriodicCheckPackageUpdatesQueryHandlerTests
{
    private readonly ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler> _crazyReport;
    private readonly IMediator _mediator;
    private readonly PeriodicCheckPackageUpdatesQueryHandler _handler;

    private readonly IEnumerable<PackageManifestPayload> _packageVersions =
    [
        PackageExt.CreatePackageManifest("panel.auth.azure", "Azure Authentication", "2.4.1-beta.3",  "5.1.0"),
        PackageExt.CreatePackageManifest("panel.monitoring.health", "Health Monitoring", "1.8.0-rc.1",  "5.1.0"),
        PackageExt.CreatePackageManifest("panel.backup.agent", "Backup Agent", "3.0.0-alpha.7",  "5.2.0"),
        PackageExt.CreatePackageManifest("panel.notifications.slack", "Slack Notifications", "4.2.5-preview.12",  "5.1.2"),
        PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "2.0.0-preview.1")
    ];

    private readonly IEnumerable<PackageManifestPayload> _packageUpdate =
    [
        PackageExt.CreatePackageManifest("panel.auth.azure", "Azure Authentication", "2.4.1",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.notifications.slack", "Slack Notifications", "4.2.5-preview.13",  "5.1.2"),
    ];
    private readonly PeriodicCheckPackageUpdatesQuery _periodicCheckPackageUpdatesQuery;
    public PeriodicCheckPackageUpdatesQueryHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _crazyReport = Substitute.For<ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler>>();
        _handler = new PeriodicCheckPackageUpdatesQueryHandler(_mediator, _crazyReport);
        _mediator.ExecuteAsync(Arg.Any<GetPackagesLatestVersionQuery>(), Arg.Any<CancellationToken>()).Returns(_packageUpdate.ToList());
        _periodicCheckPackageUpdatesQuery = new PeriodicCheckPackageUpdatesQuery(_packageVersions.ToDictionary(p => p.Id, p => NuGetVersion.Parse(p.Version)));
    }

    [Fact]
    public async Task PeriodicCheckPackageUpdatesQuery_ShouldPass()
    {
        ICollection<PackageManifestPayload> expected = _packageUpdate.ToList();

        var result = await _handler.HandleAsync(_periodicCheckPackageUpdatesQuery, TestContext.Current.CancellationToken);
        await _mediator.Received().ExecuteAsync(Arg.Any<GetPackagesLatestVersionQuery>(), Arg.Any<CancellationToken>());
        Assert.Equivalent(expected, result);
    }

    private ICollection<PackageManifestPayload> Throw() => throw new Exception("Boom");

    [Fact]
    public async Task PeriodicCheckPackageUpdatesQuery_ShouldRethrow()
    {
        _mediator.ExecuteAsync(Arg.Any<GetPackagesLatestVersionQuery>(), Arg.Any<CancellationToken>()).Returns(_ => Throw());
        var handler = _handler.HandleAsync(_periodicCheckPackageUpdatesQuery, TestContext.Current.CancellationToken);
        await _mediator.Received().ExecuteAsync(Arg.Any<GetPackagesLatestVersionQuery>(), Arg.Any<CancellationToken>());
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
