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
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using static LunaticPanel.Engine.Keys.System.PluginKeys;
using System.Text.Json.Nodes;
using LunaticPanel.Core.Abstraction.Messaging.Common;
namespace LunaticPanel.PackageManager.Tests.Application.Pulses;

public class PackageRollbackTests : PulsesBaseTests
{
    private readonly IQueryBus _queryBus;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<PackageManagerDiskEditorState> _packageManagerDiskEditorState;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;
    private readonly IServiceProvider _sp;
    public PackageRollbackTests() : base()
    {
        _queryBus = Substitute.For<IQueryBus>();
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _services.AddScoped((sp) => _queryBus);

        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
        _packageManagerDiskState = _sp.GetRequiredService<IStateAccessor<PackageManagerDiskState>>();
        _packageManagerDiskEditorState = _sp.GetRequiredService<IStateAccessor<PackageManagerDiskEditorState>>();
    }
    private readonly List<PackagePayload> _installedPackages =
          [
            PackageExt.CreatePackage("panel.auth.azure", "Azure Authentication", "2.4.0", "5.1.0"),
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.7.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.backup.agent", "Backup Agent", "2.9.0",  "5.2.0"),
            PackageExt.CreatePackage("panel.notifications.slack", "Slack Notifications", "4.2.4", "2.0.0"),
            PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "0.9.0",  "5.2.0-preview.1")
          ];


    [Fact]
    public async Task PackageRollbackAction_ShouldPass()
    {
        IEnumerable<PackagePayload> expected = new List<PackagePayload>()
        {
            _installedPackages.First()
        };
        await _dispatcher.Prepare<PackageRollbackAction>()
            .With(p => p.Package, _installedPackages.First())
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await _mediator.Received().ExecuteAsync(Arg.Any<PackageRollbackCommand>(), Arg.Any<CancellationToken>());

        Assert.False(_packageManagerDiskEditorState.State.IsLoading);
        Assert.Equivalent(expected, _packageManagerDiskEditorState.State.PendingRollbacks);

    }

    [Fact]
    public async Task PackageRollbackAction_ShouldThrow()
    {
        var diskPackage = new DiskPackagesPayload()
        {
            AvailableRollbacks = _installedPackages,
            Installed = _installedPackages,
            PendingDelete = _installedPackages,
            PendingUpdates = _installedPackages,
            PreInstalled = _installedPackages,
            RuntimePackages = _installedPackages
        };
        _mediator.ExecuteAsync(Arg.Any<PackageRollbackCommand>(), Arg.Any<CancellationToken>()).Throws(new Exception("Random Error"));
        var handler = _dispatcher.Prepare<PackageRollbackAction>()
            .With(p => p.Package, _installedPackages.First())
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
        Assert.False(_packageManagerDiskEditorState.State.IsLoading);

    }

}
