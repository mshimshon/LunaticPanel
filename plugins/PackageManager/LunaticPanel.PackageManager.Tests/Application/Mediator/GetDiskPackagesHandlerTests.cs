using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class GetDiskPackagesHandlerTests
{
    private readonly ICrazyReport<SearchRepositoryHandler> _crazyReport;
    private readonly IHostDiskPackageService _hostDiskPackageService;
    private readonly GetDiskPackagesHandler _handler;
    private readonly ICollection<PackagePayload> _installedPackages =
      [
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.7.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.backup.agent", "Backup Agent", "2.9.0",  "5.2.0"),
            PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "0.9.1",  "5.2.0-preview.1")
      ];
    private readonly ICollection<PackagePayload> _preInstalledPackages =
    [
        PackageExt.CreatePackage("panel.notifications.slack", "Slack Notifications", "4.2.4", "2.0.0"),
    ];

    private readonly ICollection<PackagePayload> _pendingInstall =
    [
        PackageExt.CreatePackage("panel.auth.azure", "Azure Authentication", "2.4.0", "5.1.0"),
    ];
    private readonly ICollection<PackagePayload> _pendingDelete =
    [
        PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "0.9.1",  "5.2.0-preview.1")
    ];

    private readonly ICollection<PackagePayload> _pendingUpdate =
[
        PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.7.1",  "5.1.0"),
    ];
    private readonly ICollection<PackagePayload> _rollbacks =
    [
        PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "0.9.0",  "5.2.0-preview.1")
    ];

    private readonly ICollection<PackageEntity> _runtimePackage =
    [
        PackageExt.CreatePackageEntity("panel.analytics.reports", "Analytics Reports", "0.9.0",  "5.2.0-preview.1", PackageManager.Domain.Entities.Enums.PackageState.Enabled, "https://source.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote)
    ];
    public GetDiskPackagesHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<SearchRepositoryHandler>>();
        _hostDiskPackageService = Substitute.For<IHostDiskPackageService>();
        _handler = new GetDiskPackagesHandler(_hostDiskPackageService, _crazyReport);
        _hostDiskPackageService.GetPendingUpdates(Arg.Any<CancellationToken>()).Returns(_pendingUpdate);
        _hostDiskPackageService.GetRollbacks(Arg.Any<CancellationToken>()).Returns(_rollbacks);
        _hostDiskPackageService.GetPendingDelete(Arg.Any<CancellationToken>()).Returns(_pendingDelete);
        _hostDiskPackageService.GetIntalled(Arg.Any<CancellationToken>()).Returns(_pendingInstall);
        _hostDiskPackageService.GetPreInstalled(Arg.Any<CancellationToken>()).Returns(_preInstalledPackages);
        _hostDiskPackageService.GetAll(Arg.Any<CancellationToken>()).Returns(_ => _runtimePackage);
    }

    [Fact]
    public async Task GetDiskPackagesQuery_ShouldPass()
    {

        var result = await _handler.HandleAsync(new GetDiskPackagesQuery(), TestContext.Current.CancellationToken);
        await _hostDiskPackageService.Received().GetPendingUpdates(Arg.Any<CancellationToken>());
        await _hostDiskPackageService.Received().GetRollbacks(Arg.Any<CancellationToken>());
        await _hostDiskPackageService.Received().GetPendingDelete(Arg.Any<CancellationToken>());
        await _hostDiskPackageService.Received().GetIntalled(Arg.Any<CancellationToken>());
        await _hostDiskPackageService.Received().GetPreInstalled(Arg.Any<CancellationToken>());
        await _hostDiskPackageService.Received().GetAll(Arg.Any<CancellationToken>());
        Assert.Collection(result.AvailableRollbacks, p => _rollbacks.Any(x => x.Info.PackageId == p.Info.PackageId));
        Assert.Collection(result.Installed, p => _installedPackages.Any(x => x.Info.PackageId == p.Info.PackageId));
        Assert.Collection(result.PendingDelete, p => _pendingDelete.Any(x => x.Info.PackageId == p.Info.PackageId));
        Assert.Collection(result.PendingUpdates, p => _pendingUpdate.Any(x => x.Info.PackageId == p.Info.PackageId));
        Assert.Collection(result.PreInstalled, p => _preInstalledPackages.Any(x => x.Info.PackageId == p.Info.PackageId));
        Assert.Collection(result.RuntimePackages, p => _runtimePackage.Any(x => x.Info.Id.Value == p.Info.PackageId));
    }
    private ICollection<PackageEntity> Throw() => throw new Exception("Boom");

    [Fact]
    public async Task GetDiskPackagesQuery_ShouldRethrow()
    {
        _hostDiskPackageService.GetAll(Arg.Any<CancellationToken>()).Returns(_ => Throw());
        var handler = _handler.HandleAsync(new GetDiskPackagesQuery(), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
