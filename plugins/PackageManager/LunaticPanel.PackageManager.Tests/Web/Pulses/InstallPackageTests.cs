using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Logging;
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

public class InstallPackageTests : PulsesBaseTests
{
    private readonly IDispatcher _dispatcher;
    private readonly IServiceProvider _sp;
    private readonly IStateAccessor<PackageManagerDiskState> _diskState;
    public InstallPackageTests() : base()
    {
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
        _diskState = _sp.GetRequiredService<IStateAccessor<PackageManagerDiskState>>();
    }
    private readonly ICollection<RepositorySourceEntity> _packageSources =
      [
            PackageExt.CreateRepositorySourceEntity("Source A", "http://sss.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote, PackageManager.Domain.Entities.Enums.RepositorySourceState.Enabled),
            PackageExt.CreateRepositorySourceEntity("Source B", "http://sss3.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote, PackageManager.Domain.Entities.Enums.RepositorySourceState.Disabled),
      ];

    private readonly IEnumerable<PackageManifestPayload> _install =
    [
        PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "2.2.0-preview.1"),
    ];
    private readonly IEnumerable<PackagePayload> _installedPackages =
   [
            PackageExt.CreatePackage("panel.notifications.slack", "Slack Notifications", "4.2.4", "2.0.0"),
            PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "2.2.0-preview.1")
   ];
    [Fact]
    public async Task AutoUpdatePackagesAction_ShouldPass()
    {
        var stateInstallResult = _installedPackages.ToList();
        _mediator.ExecuteAsync(Arg.Any<GetDiskPackagesQuery>(), Arg.Any<CancellationToken>()).Returns(new DiskPackagesPayload() { Installed = stateInstallResult });
        await _dispatcher.Prepare<InstallPackageAction>()
            .With(p => p.Target, _install.First())
            .Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        await _mediator.Received().ExecuteAsync(Arg.Any<PackageInstallCommand>(), Arg.Any<CancellationToken>());
        Assert.Equivalent(stateInstallResult, _diskState.State.Installed);
    }

    [Fact]
    public async Task AutoUpdatePackagesAction_ShouldThrow()
    {
        _mediator.ExecuteAsync(Arg.Any<PackageInstallCommand>(), Arg.Any<CancellationToken>()).Throws(new Exception("Random Error"));
        var handler = _dispatcher.Prepare<InstallPackageAction>().Await().DispatchAsync(TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }


}
