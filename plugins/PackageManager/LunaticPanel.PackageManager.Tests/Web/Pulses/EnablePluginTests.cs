using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
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
namespace LunaticPanel.PackageManager.Tests.Web.Pulses;

public class EnablePluginTests : PulsesBaseTests
{
    private readonly IEventBus _eventBus;
    private readonly IDispatcher _dispatcher;
    private readonly IServiceProvider _sp;
    public EnablePluginTests() : base()
    {
        _eventBus = Substitute.For<IEventBus>();
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _services.AddScoped((sp) => _eventBus);

        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
    }



    [Fact]
    public async Task EnablePluginAction_ShouldPass()
    {
        await _dispatcher.Prepare<EnablePluginAction>()
            .With(p => p.Id, "fddfd.vvv")
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await _eventBus.Received().PublishAsync(Arg.Any<IEventBusMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EnablePluginAction_ShouldThrow()
    {
        _eventBus.PublishAsync(Arg.Any<IEventBusMessage>(), Arg.Any<CancellationToken>()).Throws(new Exception("Random Error"));
        var handler = _dispatcher.Prepare<EnablePluginAction>()
            .With(p => p.Id, "fddfd.vvv")
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }


}
