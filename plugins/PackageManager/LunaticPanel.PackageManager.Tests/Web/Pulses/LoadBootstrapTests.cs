using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Logging;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.Effects;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using LunaticPanel.PackageManager.Tests.Application.Mock;
using LunaticPanel.PackageManager.Tests.Web.Pulses.Utils;
using MaksimShimshon.Mediator;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using NuGet.Versioning;
using StatePulse.Net;
using System;
using System.Text.Json.Nodes;
using static LunaticPanel.Engine.Keys.System.PluginKeys;
namespace LunaticPanel.PackageManager.Tests.Web.Pulses;

public class LoadBootstrapTests : PulsesBaseTests
{
    private readonly IQueryBus _queryBus;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<BootstrapState> _bootstrapState;
    private readonly IServiceProvider _sp;
    public LoadBootstrapTests() : base()
    {
        _queryBus = Substitute.For<IQueryBus>();
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _services.AddScoped((sp) => _queryBus);

        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
        _bootstrapState = _sp.GetRequiredService<IStateAccessor<BootstrapState>>();
    }


    private readonly List<ExternalPluginPayload> _mockResult = [
        new ExternalPluginPayload() {
            StartupState = ExternalPluginStartupState.Enabled ,
            Manifest = new(){
                Id = "Balooo"
            }
        },
        new ExternalPluginPayload() {
            StartupState = ExternalPluginStartupState.Disabled ,
            Manifest = new(){
                Id = "Boolaaaa"
            }
        }
    ];

    [Fact]
    public async Task LoadBootstrapAction_ShouldPass()
    {
        List<string> enabled = _mockResult
            .Where(x => x.StartupState == ExternalPluginStartupState.Enabled)
            .Select(x => x.Id)
            .ToList();

        List<string> disabled = _mockResult
            .Where(x => x.StartupState == ExternalPluginStartupState.Disabled)
            .Select(x => x.Id)
            .ToList();

        _queryBus
            .QueryAsync(Arg.Is<QueryBusMessage>(m => m.Key.SameAs(Queries.FETCH_BOOTSTRAP_FILE)), Arg.Any<CancellationToken>())
            .Returns(c => new QueryBusMessageResponse(new(_mockResult)));
        await _dispatcher.Prepare<LoadBootstrapAction>()
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await _queryBus.Received().QueryAsync(Arg.Is<QueryBusMessage>(m => m.Key.SameAs(Queries.FETCH_BOOTSTRAP_FILE)), Arg.Any<CancellationToken>());
        Assert.Equivalent(enabled, _bootstrapState.State.Enabled);
        Assert.Equivalent(disabled, _bootstrapState.State.Disabled);

    }

    [Fact]
    public async Task LoadBootstrapAction_ShouldThrow()
    {

        _queryBus.QueryAsync(Arg.Is<QueryBusMessage>(m => m.Key.SameAs(Queries.FETCH_BOOTSTRAP_FILE)), Arg.Any<CancellationToken>()).Throws(new Exception("Random Error"));
        var handler = _dispatcher.Prepare<LoadBootstrapAction>()
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }

    [Fact]
    public async Task LoadBootstrapAction_ShouldThrowWhenDefaultResult()
    {

        _queryBus
            .QueryAsync(Arg.Is<QueryBusMessage>(m => m.Key.SameAs(Queries.FETCH_BOOTSTRAP_FILE)), Arg.Any<CancellationToken>())
            .Returns(c => new QueryBusMessageResponse());
        var handler = _dispatcher.Prepare<LoadBootstrapAction>()
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }


}
