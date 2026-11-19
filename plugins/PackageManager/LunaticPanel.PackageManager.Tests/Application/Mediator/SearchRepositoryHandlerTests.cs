using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class SearchRepositoryHandlerTests
{
    private readonly ICrazyReport<SearchRepositoryHandler> _crazyReport;
    private readonly IPackageDownloader _packageDownloader;
    private readonly SearchRepositoryHandler _handler;
    private readonly IEnumerable<PackageManifestPayload> _updateFound =
    [
        PackageExt.CreatePackageManifest("panel.auth.azure", "Azure Authentication", "2.4.1-beta.3",  "5.1.0"),
        PackageExt.CreatePackageManifest("panel.monitoring.health", "Health Monitoring", "1.8.0-rc.1",  "5.1.0"),
        PackageExt.CreatePackageManifest("panel.backup.agent", "Backup Agent", "3.0.0-alpha.7",  "5.2.0"),
        PackageExt.CreatePackageManifest("panel.notifications.slack", "Slack Notifications", "4.2.5-preview.12",  "5.1.2"),
        PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "2.0.0-preview.1")
    ];

    private readonly ICollection<RepositorySourcePayload> _sources =
    [
        PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled)
    ];
    public SearchRepositoryHandlerTests()
    {
        _packageDownloader = Substitute.For<IPackageDownloader>();
        _crazyReport = Substitute.For<ICrazyReport<SearchRepositoryHandler>>();
        _packageDownloader.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>())
            .Returns(CallInfo =>
        {
            return _sources
                .ToDictionary(p => p,
                p => new SearchResponse<PackageManifestPayload>()
                {
                    Position = 0,
                    TotalResults = _updateFound.Count(),
                    Result = _updateFound
                });
        });
        _handler = new SearchRepositoryHandler(_packageDownloader, _crazyReport);
    }

    [Fact]
    public async Task SearchRepositoryQuery_ShouldPass()
    {
        var expected = _sources
                .ToDictionary(p => p,
                p => new SearchResponse<PackageManifestPayload>()
                {
                    Position = 0,
                    TotalResults = _updateFound.Count(),
                    Result = _updateFound
                });
        var result = await _handler.HandleAsync(new SearchRepositoryQuery() { Search = new() { Keywords = "my key word" } }, TestContext.Current.CancellationToken);
        await _packageDownloader.Received().SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>());
    }

    private Dictionary<RepositorySourcePayload, SearchResponse<PackageManifestPayload>> Throw<TException>(Exception ex) => throw new Exception("Boom");

    [Fact]
    public async Task SearchRepositoryQuery_ShouldRethrow()
    {
        _packageDownloader.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>()).Throws<Exception>();
        var handler = _handler.HandleAsync(new SearchRepositoryQuery() { Search = new() { Keywords = "my key word", MaxResult = 30 } }, TestContext.Current.CancellationToken);
        await _packageDownloader.Received().SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>());
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }

    [Fact]
    public async Task SearchRepositoryQuery_ShouldRethrow_HostCodedException()
    {
        _packageDownloader.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>()).Throws(new HostCodedException("errorCode", "errorMessage"));
        var handler = _handler.HandleAsync(new SearchRepositoryQuery() { Search = new() { Keywords = "my key word" } }, TestContext.Current.CancellationToken);
        await _packageDownloader.Received().SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>());
        await Assert.ThrowsAsync<HostCodedException>(() => handler);
    }

    [Theory]
    [InlineData(100, 50)]
    [InlineData(-1, 10)]
    public async Task SearchRequest_ShouldFallbackMaxResult(int maxResult, int expectedMaxResult)
    {
        var expected = new SearchRequest()
        {
            MaxResult = maxResult,
        };
        Assert.Equal(expectedMaxResult, expected.MaxResult);
    }
}
