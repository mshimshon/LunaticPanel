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
using NSubstitute.Core.Arguments;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class GetPackageVersionsHandlerTests
{
    private readonly IPackageDownloader _packageDownloader;
    private readonly GetPackageVersionsHandler _handler;
    private readonly ICollection<PackagePayload> _packagesInSource =
      [
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.7.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.3.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.0.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.monitoring.healthe", "Health Monitoring", "1.0.0",  "5.1.0"),
      ];

    private readonly ICollection<RepositorySourcePayload> _packageSources =
    [
        PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled),
    ];
    public GetPackageVersionsHandlerTests()
    {
        _packageDownloader = Substitute.For<IPackageDownloader>();
        _packageDownloader.GetVersionsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(callInfo =>
        {
            var packageNames = callInfo.Arg<string>();
            var filteredResult = _packagesInSource.Where(p => p.Info.PackageId.Equals(packageNames, StringComparison.OrdinalIgnoreCase)).Select(p => p.Version);
            return Task.FromResult(filteredResult);
        });
        _handler = new GetPackageVersionsHandler(_packageDownloader);
    }

    [Fact]
    public async Task PackageRollbackCommand_ShouldPass()
    {
        var expected = _packagesInSource.Where(p => p.Info.PackageId.Equals("panel.monitoring.health", StringComparison.OrdinalIgnoreCase)).Select(p => p.Version).ToList();
        var result = await _handler.HandleAsync(new GetPackageVersionsQuery("panel.monitoring.health", _packageSources), TestContext.Current.CancellationToken);
        await _packageDownloader.Received().GetVersionsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        Assert.Equivalent(expected, result, strict: true);
    }
    private IEnumerable<string> Throw() => throw new Exception("Boom");
    [Fact]
    public async Task PackageRollbackCommand_ShouldRethrow()
    {
        _packageDownloader.GetVersionsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_ => Throw());
        var handler = _handler.HandleAsync(new GetPackageVersionsQuery("panel.monitoring.health", _packageSources), TestContext.Current.CancellationToken);
        await _packageDownloader.Received().GetVersionsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
