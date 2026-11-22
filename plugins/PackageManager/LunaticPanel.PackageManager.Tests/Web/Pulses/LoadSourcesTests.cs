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

public class LoadSourcesTests : PulsesBaseTests
{
    private readonly IEventBus _eventBus;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<RepositorySourceState> _sourceState;
    private readonly IServiceProvider _sp;
    public LoadSourcesTests() : base()
    {
        _eventBus = Substitute.For<IEventBus>();
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _services.AddScoped((sp) => _eventBus);

        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
        _sourceState = _sp.GetRequiredService<IStateAccessor<RepositorySourceState>>();

    }
    private readonly ICollection<RepositorySourceEntity> _packageSources =
      [
            PackageExt.CreateRepositorySourceEntity("Source A", "http://sss.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote, PackageManager.Domain.Entities.Enums.RepositorySourceState.Enabled),
            PackageExt.CreateRepositorySourceEntity("Source B", "http://sss3.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote, PackageManager.Domain.Entities.Enums.RepositorySourceState.Disabled),
      ];



    [Fact]
    public async Task LoadSourcesAction_ShouldPass()
    {
        var payloads = _packageSources.Select(p => p.ToApplicationPayload());
        _mediator.ExecuteAsync(Arg.Any<GetSourcesQuery>(), Arg.Any<CancellationToken>()).Returns(payloads.ToList());
        await _dispatcher.Prepare<LoadSourcesAction>()
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        Assert.Equivalent(payloads, _sourceState.State.Sources);
    }

    [Fact]
    public async Task LoadSourcesAction_ShouldThrow()
    {

        _mediator.ExecuteAsync(Arg.Any<GetSourcesQuery>(), Arg.Any<CancellationToken>()).Throws(new Exception("Random Error"));
        var handler = _dispatcher.Prepare<LoadSourcesAction>()
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }



    [Fact]
    public async Task LoadSourcesAction_ShouldThrowHostCodedException()
    {

        _mediator.ExecuteAsync(Arg.Any<GetSourcesQuery>(), Arg.Any<CancellationToken>()).Throws(new HostCodedException("ccc", "Random Error"));
        var handler = _dispatcher.Prepare<LoadSourcesAction>()
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostCodedException>(() => handler);

    }


}
