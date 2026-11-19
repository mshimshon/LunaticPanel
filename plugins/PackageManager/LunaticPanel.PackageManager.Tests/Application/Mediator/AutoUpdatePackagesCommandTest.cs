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
namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class AutoUpdatePackagesCommandTest
{
    private readonly IPanelControl _panelControl;
    private readonly IMediator _mediator;
    private readonly IStateAccessor<PackageUpdateState> _packageUpdateState;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;
    private readonly ICrazyReport<AutoUpdatePackagesCommandHandler> _crazyReport;


    private readonly AutoUpdatePackagesCommandHandler _handler;
    public AutoUpdatePackagesCommandTest()
    {
        _panelControl = Substitute.For<IPanelControl>();
        _mediator = Substitute.For<IMediator>();

        _packageUpdateState =
            Substitute.For<IStateAccessor<PackageUpdateState>>();

        _packageManagerDiskState =
            Substitute.For<IStateAccessor<PackageManagerDiskState>>();

        _crazyReport =
            Substitute.For<ICrazyReport<AutoUpdatePackagesCommandHandler>>();

        _handler = new AutoUpdatePackagesCommandHandler(
            _panelControl,
            _mediator,
            _crazyReport);

    }

    [Fact]
    public async Task Pipeline_NoPackagesShouldUpdate()
    {
        IEnumerable<PackageManifestPayload> _updateFound = [
            PackageExt.CreatePackageManifest("panel.auth.azure", "Azure Authentication", "2.4.1-beta.3",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.monitoring.health", "Health Monitoring", "1.8.0-rc.1",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.backup.agent", "Backup Agent", "3.0.0-alpha.7",  "5.2.0"),
            PackageExt.CreatePackageManifest("panel.notifications.slack", "Slack Notifications", "4.2.5-preview.12",  "5.1.2"),
            PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "5.2.0-preview.1")
        ];
        var updateState = new PackageUpdateState() { FoundUpdates = _updateFound };
        IEnumerable<PackagePayload> _installedPackages = [
            PackageExt.CreatePackage("panel.auth.azure", "Azure Authentication", "2.4.0", "5.1.0"),
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.7.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.backup.agent", "Backup Agent", "2.9.0",  "5.2.0"),
            PackageExt.CreatePackage("panel.notifications.slack", "Slack Notifications", "4.2.4", "5.1.2"),
            PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "0.9.0",  "5.2.0-preview.1")
        ];
        var diskState = new PackageManagerDiskState()
        {
            Installed = _installedPackages,
            IsInitialized = true,
        };
        _panelControl.PanelVersion.Returns("2.0.0");
        _packageUpdateState.State.Returns(updateState);
        _packageManagerDiskState.State.Returns(diskState);
        var currentlyInstalledVersions = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => p.Version);
        var foundUpdates = _packageUpdateState.State.FoundUpdates.ToList();
        await _handler.HandleAsync(new AutoUpdatePackagesCommand(foundUpdates, currentlyInstalledVersions), TestContext.Current.CancellationToken);


        // Assert
        await _mediator.Received(0)
            .ExecuteAsync(
                Arg.Any<PackageUpdateCommand>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Pipeline_OnePackagesShouldUpdate()
    {
        IEnumerable<PackageManifestPayload> _updateFound =
        [
            PackageExt.CreatePackageManifest("panel.auth.azure", "Azure Authentication", "2.4.1-beta.3",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.monitoring.health", "Health Monitoring", "1.8.0-rc.1",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.backup.agent", "Backup Agent", "3.0.0-alpha.7",  "5.2.0"),
            PackageExt.CreatePackageManifest("panel.notifications.slack", "Slack Notifications", "4.2.5-preview.12",  "5.1.2"),
            PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "2.0.0-preview.1")
        ];
        var updateState = new PackageUpdateState() { FoundUpdates = _updateFound };
        IEnumerable<PackagePayload> _installedPackages =
        [
            PackageExt.CreatePackage("panel.auth.azure", "Azure Authentication", "2.4.0", "5.1.0"),
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.7.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.backup.agent", "Backup Agent", "2.9.0",  "5.2.0"),
            PackageExt.CreatePackage("panel.notifications.slack", "Slack Notifications", "4.2.4", "2.0.0"),
            PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "0.9.0",  "5.2.0-preview.1")
        ];
        var diskState = new PackageManagerDiskState()
        {
            Installed = _installedPackages,
            IsInitialized = true,
        };
        _panelControl.PanelVersion.Returns("2.0.0");
        _packageUpdateState.State.Returns(updateState);
        _packageManagerDiskState.State.Returns(diskState);
        var currentlyInstalledVersions = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => p.Version);
        var foundUpdates = _packageUpdateState.State.FoundUpdates.ToList();
        await _handler.HandleAsync(new AutoUpdatePackagesCommand(foundUpdates, currentlyInstalledVersions), TestContext.Current.CancellationToken);


        // Assert
        await _mediator.Received(1)
            .ExecuteAsync(
                Arg.Any<PackageUpdateCommand>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Pipeline_HandlingReThrow()
    {
        IEnumerable<PackageManifestPayload> _updateFound =
        [
            PackageExt.CreatePackageManifest("panel.analytics.reports", "Analytics Reports", "1.0.0-nightly.45",  "2.2.0-preview.1"),
            PackageExt.CreatePackageManifest("panel.auth.azure", "Azure Authentication", "2.4.1-beta.3",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.monitoring.health", "Health Monitoring", "1.8.0-rc.1",  "5.1.0"),
            PackageExt.CreatePackageManifest("panel.backup.agent", "Backup Agent", "3.0.0-alpha.7",  "5.2.0"),
            PackageExt.CreatePackageManifest("panel.notifications.slack", "Slack Notifications", "4.2.5-preview.12",  "5.1.2")
        ];
        var updateState = new PackageUpdateState() { FoundUpdates = _updateFound };
        IEnumerable<PackagePayload> _installedPackages =
        [
            PackageExt.CreatePackage("panel.auth.azure", "Azure Authentication", "2.4.0", "5.1.0"),
            PackageExt.CreatePackage("panel.monitoring.health", "Health Monitoring", "1.7.0",  "5.1.0"),
            PackageExt.CreatePackage("panel.backup.agent", "Backup Agent", "2.9.0",  "5.2.0"),
            PackageExt.CreatePackage("panel.notifications.slack", "Slack Notifications", "4.2.4", "2.0.0"),
            PackageExt.CreatePackage("panel.analytics.reports", "Analytics Reports", "0.9.0",  "5.2.0-preview.1")
        ];
        var diskState = new PackageManagerDiskState()
        {
            Installed = _installedPackages,
            IsInitialized = true,
        };
        _panelControl.PanelVersion.Returns("2.2.0");
        _packageUpdateState.State.Returns(updateState);
        _packageManagerDiskState.State.Returns(diskState);
        _mediator.ExecuteAsync(
        Arg.Any<PackageUpdateCommand>(),
        Arg.Any<CancellationToken>()).Throws(new Exception("Boom"));
        var currentlyInstalledVersions = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => p.Version);
        var foundUpdates = _packageUpdateState.State.FoundUpdates.ToList();
        var handler = _handler.HandleAsync(new AutoUpdatePackagesCommand(foundUpdates, currentlyInstalledVersions), TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<HostUnkownException>(() => handler);

    }

    [Theory]
    [InlineData("1.0.0", "1.1.0", "1.0.0", "1.0.0")]
    [InlineData("1.0.0", "1.2.0", "1.1.0", "1.5.0")]
    [InlineData("2.5.0", "2.10.0", "2.9.0", "2.0.0")]
    [InlineData("3.0.0", "3.0.1", "3.0.0", "3.9.9")]
    [InlineData("5.1.0", "5.2.0", "5.1.9", "5.0.0")]

    // prerelease panel versions
    [InlineData("1.0.0", "1.0.0-preview.2", "1.0.0-preview.1", "1.0.0")]
    [InlineData("2.0.0", "2.0.0-rc.1", "2.0.0-beta.5", "2.1.0")]
    [InlineData("3.0.0-preview.1", "3.1.0-preview.2", "3.1.0-preview.1", "3.0.0")]
    public void Validation_ShouldReturnFalse_WhenPackagePanelVersionIsNewerThanCurrentPanelVersion(
        string packageVersion,
        string packagePanelVersion,
        string currentPanelVersion,
        string currentVersion)
    {
        // Arrange
        var payload = new PackageManifestPayload
        {
            Id = "TestPackage",
            Version = packageVersion,
            PanelVersion = packagePanelVersion
        };

        // Act
        var result = AutoUpdatePackagesCommandHandler.Validation(
            payload,
            NuGetVersion.Parse(currentPanelVersion),
            null);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", "1.0.0", "1.0.0")]
    [InlineData("1.2.0", "1.0.0", "1.1.0", "1.5.0")]
    [InlineData("1.0.1", "1.0.0", "1.0.0", "1.0.0")]
    [InlineData("1.99.99", "1.0.0", "1.99.99", "1.0.0")]
    [InlineData("5.10.3", "5.0.0", "5.8.0", "5.999.999")]
    [InlineData("10.0.0", "10.0.0", "10.0.0", "10.0.0")]
    [InlineData("3.4.5", "3.0.0", "3.4.5", "3.9.9")]

    // prerelease versions
    [InlineData("1.0.0-preview.1", "1.0.0", "1.0.0", "1.0.0")]
    [InlineData("1.2.0-preview.5", "1.0.0-preview.1", "1.1.0", "1.5.0")]
    [InlineData("2.0.0-rc.1", "2.0.0-preview.1", "2.0.0", "2.1.0")]
    [InlineData("3.1.0-beta", "3.0.0", "3.0.0", "3.5.0")]
    [InlineData("4.0.0-preview.999", "4.0.0", "4.1.0", "4.10.0")]
    public void Validation_ShouldReturnTrue_WhenAllChecksPass(
        string packageVersion,
        string packagePanelVersion,
        string currentPanelVersion,
        string currentVersion)
    {
        // Arrange
        var payload = new PackageManifestPayload
        {
            Id = "TestPackage",
            Version = packageVersion,
            PanelVersion = packagePanelVersion
        };

        // Act
        var result = AutoUpdatePackagesCommandHandler.Validation(
            payload,
            NuGetVersion.Parse(currentPanelVersion),
            null);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("1.0.0", "1.0.0", "1.0.0", "1.0.0")]
    [InlineData("1.1.0", "1.2.0", "1.2.0", "1.0.0")]
    [InlineData("2.5.0", "2.9.9", "2.9.9", "2.0.0")]
    [InlineData("5.10.3", "5.0.0", "5.0.0", "5.999.999")]

    // prerelease panel versions match exactly
    [InlineData("1.0.0-preview.1", "1.0.0-preview.1", "1.0.0-preview.1", "1.0.0")]
    [InlineData("2.0.0", "2.0.0-rc.1", "2.0.0-rc.1", "2.1.0")]
    [InlineData("3.1.0-beta", "3.0.0-preview.5", "3.0.0-preview.5", "3.9.0")]

    // stable panel version equals stable panel version
    [InlineData("4.0.0-preview.1", "4.0.0", "4.0.0", "4.5.0")]
    public void Validation_ShouldReturnTrue_WhenPackagePanelVersionEqualsCurrentPanelVersion(
        string packageVersion,
        string packagePanelVersion,
        string currentPanelVersion,
        string currentVersion)
    {
        // Arrange
        var payload = new PackageManifestPayload
        {
            Id = "TestPackage",
            Version = packageVersion,
            PanelVersion = packagePanelVersion
        };

        // Act
        var result = AutoUpdatePackagesCommandHandler.Validation(
            payload,
            NuGetVersion.Parse(currentPanelVersion),
            null);

        // Assert
        Assert.True(result);
    }
}
