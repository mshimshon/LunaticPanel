using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Tests.Application.Mock;
using StatePulse.Net;
using System;
using LunaticPanel.Core.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using LunaticPanel.PackageManager.Application.Payloads;
namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class AutoUpdatePackagesCommandTest
{
    public AutoUpdatePackagesCommandTest() { }

    [Fact]
    public void Validation_ShouldReturnFalse_WhenPanelMajorVersionMismatch()
    {
        // Arrange
        var payload = new PackageManifestPayload
        {
            Id = "TestPackage",
            Version = "1.2.3",
            PanelVersion = "2.0.0"
        };

        var currentVersion = new NuGetVersion("1.0.0");
        var currentPanelVersion = new NuGetVersion("1.5.0");


        // Act
        var result = AutoUpdatePackagesCommandHandler.Validation(
            payload,
            currentVersion,
            currentPanelVersion, null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validation_ShouldReturnFalse_WhenPackageMajorVersionMismatch()
    {
        // Arrange
        var payload = new PackageManifestPayload
        {
            Id = "TestPackage",
            Version = "2.0.0",
            PanelVersion = "1.0.0"
        };

        var currentVersion = new NuGetVersion("1.0.0");
        var currentPanelVersion = new NuGetVersion("1.0.0");

        // Act
        var result = AutoUpdatePackagesCommandHandler.Validation(
            payload,
            currentVersion,
            currentPanelVersion, null);

        // Assert
        Assert.False(result);
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
            new NuGetVersion(currentVersion),
            new NuGetVersion(currentPanelVersion),
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
            new NuGetVersion(currentVersion),
            new NuGetVersion(currentPanelVersion),
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
            new NuGetVersion(currentVersion),
            new NuGetVersion(currentPanelVersion),
            null);

        // Assert
        Assert.True(result);
    }
}
