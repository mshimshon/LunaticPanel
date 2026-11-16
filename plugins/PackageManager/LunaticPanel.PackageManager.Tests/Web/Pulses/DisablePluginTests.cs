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
using LunaticPanel.PackageManager.Pulses.Effects;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;
namespace LunaticPanel.PackageManager.Tests.Application.Pulses;

public class DisablePluginTests : PulsesBaseTests
{
    private readonly IEventBus _eventBus;
    private readonly IDispatcher _dispatcher;
    private readonly IServiceProvider _sp;
    public DisablePluginTests() : base()
    {
        _eventBus = Substitute.For<IEventBus>();
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _services.AddScoped((sp) => _eventBus);

        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
    }



    [Fact]
    public async Task DisablePluginAction_ShouldPass()
    {
        await _dispatcher.Prepare<DisablePluginAction>()
            .With(p => p.Id, "fddfd.vvv")
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await _eventBus.Received().PublishAsync(Arg.Any<IEventBusMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DisablePluginAction_ShouldThrow()
    {
        _eventBus.PublishAsync(Arg.Is<EventBusMessage>(p => p.Key.SameAs(PluginKeys.Events.DISABLE_PLUGIN)), Arg.Any<CancellationToken>()).Throws(new Exception("Random Error"));
        var handler = _dispatcher.Prepare<DisablePluginAction>()
            .With(p => p.Id, "fddfd.vvv")
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }


}
