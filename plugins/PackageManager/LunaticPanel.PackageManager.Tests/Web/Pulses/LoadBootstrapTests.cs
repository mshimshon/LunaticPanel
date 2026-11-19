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
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using static LunaticPanel.Engine.Keys.System.PluginKeys;
using System.Text.Json.Nodes;
using LunaticPanel.Core.Abstraction.Messaging.Common;
namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Web.Pulses;

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


    private JsonArray _jsonArrayMock = new JsonArray(
        new JsonObject
        {
            ["Entity"] = new JsonObject
            {
                ["Identity"] = new JsonObject
                {
                    ["PackageId"] = "com.example.enabledapp"
                },
                ["Lifecycle"] = new JsonObject
                {
                    ["StartupState"] = 1
                }
            }
        },
        new JsonObject
        {
            ["Entity"] = new JsonObject
            {
                ["Identity"] = new JsonObject
                {
                    ["PackageId"] = "com.example.disabledapp"
                },
                ["Lifecycle"] = new JsonObject
                {
                    ["StartupState"] = 0
                }
            }
        },
        new JsonObject
        {
            ["Entity"] = new JsonObject
            {
                ["Identity"] = new JsonObject
                {
                    ["PackageId"] = "com.example.otherstate"
                },
                ["Lifecycle"] = new JsonObject
                {
                    ["StartupState"] = 2
                }
            }
        }
    );
    [Fact]
    public async Task LoadBootstrapAction_ShouldPass()
    {
        List<string> enabled = _jsonArrayMock
            .Where(x => x?["Entity"]!["Lifecycle"]!["StartupState"]?.GetValue<int>() == 1)
            .Select(x => x!["Entity"]!["Identity"]!["PackageId"]!.GetValue<string>())
            .ToList();

        List<string> disabled = _jsonArrayMock
            .Where(x => x?["Entity"]!["Lifecycle"]!["StartupState"]?.GetValue<int>() == 0)
            .Select(x => x!["Entity"]!["Identity"]!["PackageId"]!.GetValue<string>())
            .ToList();

        _queryBus
            .QueryAsync(Arg.Is<QueryBusMessage>(m => m.Key.SameAs(Queries.FETCH_BOOTSTRAP_FILE)), Arg.Any<CancellationToken>())
            .Returns(c => new QueryBusMessageResponse(new(_jsonArrayMock)));
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
