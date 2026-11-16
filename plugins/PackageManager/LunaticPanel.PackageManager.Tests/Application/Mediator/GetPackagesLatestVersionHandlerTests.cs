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

using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class GetPackagesLatestVersionHandlerTests
{
    private readonly ICrazyReport<GetPackagesLatestVersionHandler> _crazyReport;
    private readonly IPackageDownloader _packageDownloader;
    private readonly GetPackagesLatestVersionHandler _handler;
    private readonly ICollection<PackageManifestPayload> _packageResult =
    [
        PackageExt.CreatePackageManifest("panel.monitoring.health", "Health Monitoring", "1.7.0",  "5.1.0"),
        PackageExt.CreatePackageManifest("panel.backup.agent", "Backup Agent", "2.9.0",  "5.2.0"),
        PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "0.9.1",  "5.2.0-preview.1")
    ];
    private readonly ICollection<string> _packageSearch = ["panel.monitoring.health"];
    public GetPackagesLatestVersionHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<GetPackagesLatestVersionHandler>>();
        _packageDownloader = Substitute.For<IPackageDownloader>();
        _handler = new GetPackagesLatestVersionHandler(_packageDownloader);
    }

    [Fact]
    public async Task PackageRollbackCommand_ShouldPass()
    {

        _packageDownloader.GetLatestVersionAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var packageNames = callInfo.Arg<IEnumerable<string>>();
                var filteredResult = _packageResult.Where(p => packageNames.Contains(p.Id));
                return Task.FromResult(filteredResult);
            });
        var result = await _handler.HandleAsync(new GetPackagesLatestVersionQuery(_packageSearch), TestContext.Current.CancellationToken);
        Assert.True(result.Count == 1);
        Assert.True(result.First() == _packageResult.First());

    }
    private IEnumerable<PackageManifestPayload> Throw() => throw new Exception("Boom");

    [Fact]
    public async Task PackageRollbackCommand_ShouldRethrow()
    {
        _packageDownloader.GetLatestVersionAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Returns(_ => Throw());
        var handler = _handler.HandleAsync(new GetPackagesLatestVersionQuery(_packageSearch), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
