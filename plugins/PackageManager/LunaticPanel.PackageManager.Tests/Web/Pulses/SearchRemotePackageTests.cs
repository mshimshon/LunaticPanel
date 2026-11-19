using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mock;
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
using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute.ExceptionExtensions;
using LunaticPanel.PackageManager.Pulses.Effects;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Web.Pulses;

public class SearchRemotePackageTests : PulsesBaseTests
{
    private readonly IEventBus _eventBus;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<SearchPackageState> _searchPackageState;
    private readonly IServiceProvider _sp;
    public SearchRemotePackageTests() : base()
    {
        _eventBus = Substitute.For<IEventBus>();
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _services.AddScoped((sp) => _eventBus);

        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
        _searchPackageState = _sp.GetRequiredService<IStateAccessor<SearchPackageState>>();

    }
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


    [Fact]
    public async Task SearchRemotePackageAction_ShouldPass()
    {
        var mockedResponse = _sources
                .ToDictionary(p => p, p => new SearchResponse<PackageManifestPayload>()
                {
                    Position = 0,
                    TotalResults = _updateFound.Count(),
                    Result = _updateFound
                });
        _mediator.ExecuteAsync(Arg.Any<SearchRepositoryQuery>(), Arg.Any<CancellationToken>()).Returns(CallInfo =>
        {
            return mockedResponse;
        });
        await _dispatcher.Prepare<SearchRemotePackageAction>()
            .With(p => p.Keywords, "my key words")
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        Assert.False(_searchPackageState.State.IsLoading);
        Assert.Equivalent(mockedResponse, _searchPackageState.State.Search);
    }

    [Fact]
    public async Task SearchRemotePackageAction_ShouldPassAntiDuplication()
    {
        List<Task> backgroundRunners = new();
        _mediator.ExecuteAsync(Arg.Any<SearchRepositoryQuery>(), Arg.Any<CancellationToken>()).Returns(callInfo =>
        {
            var query = callInfo.Arg<SearchRepositoryQuery>();
            return _sources.ToDictionary(p => p with { Name = query.Search.Keywords }, p => new SearchResponse<PackageManifestPayload>()
            {
                Position = 0,
                TotalResults = _updateFound.Count(),
                Result = _updateFound
            });
        });
        // Simulate Typing Burst
        for (int i = 0; i < 25; i++)
        {
            var current = i;

            var t = _dispatcher.Prepare<SearchRemotePackageAction>()
                .With(p => p.Keywords, current.ToString())
                .Await()
                .DispatchAsync(TestContext.Current.CancellationToken);
            backgroundRunners.Add(t);
        }
        await Task.WhenAll(backgroundRunners);
        Assert.False(_searchPackageState.State.IsLoading);
        Assert.Equivalent("24", _searchPackageState.State.Search.First().Key.Name);
    }

    [Fact]
    public async Task SearchRemotePackageAction_ShouldThrow()
    {

        var payload = PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled);
        _mediator.ExecuteAsync(Arg.Any<SearchRepositoryQuery>(), Arg.Any<CancellationToken>()).Throws(new Exception("Random Error"));
        var handler = _dispatcher.Prepare<SearchRemotePackageAction>()
            .With(p => p.Keywords, "sda sd as")
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }


}
