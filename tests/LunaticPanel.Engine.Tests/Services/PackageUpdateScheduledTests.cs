using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Hooks.Events.Scheduled;
using LunaticPanel.Engine.Infrastructure.Interfaces;
using NSubstitute;
using NuGet.Versioning;
using System;
using System.Reflection;

namespace LunaticPanel.Engine.Tests.Services;

public class PackageUpdateScheduledTests
{
    private readonly IEventBus _eventBus;
    private readonly IPanelUpdateService _panelUpdateService;
    private readonly ICrazyReport<PackageUpdateScheduled> _crazyReport;
    private readonly PackageUpdateScheduled _packageUpdateScheduled;
    public PackageUpdateScheduledTests()
    {

        _crazyReport = Substitute.For<ICrazyReport<PackageUpdateScheduled>>();
        _panelUpdateService = Substitute.For<IPanelUpdateService>();
        _eventBus = Substitute.For<IEventBus>();
        _packageUpdateScheduled = new(_crazyReport, _panelUpdateService, _eventBus);
    }

    [Fact]
    public async Task UpdateSchedule_PanelVersion_Pass()
    {
        Environment.SetEnvironmentVariable("LP_UPDATE_HOURS", "1");
        var panelVersion = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
        Assert.Equal(panelVersion, _packageUpdateScheduled._panelVersion);
    }

    [Fact]
    public async Task UpdateSchedule_HoursEnvironment_Pass()
    {
        Environment.SetEnvironmentVariable("LP_UPDATE_HOURS", "1");
        var create = new PackageUpdateScheduled(_crazyReport, _panelUpdateService, _eventBus);
        var expected = new TimeSpan(0, 1, 0, 0);
        Assert.Equal(expected, create._configuredTiming);
    }

    [Theory]
    [InlineData("15", "1")]
    [InlineData("15", "14")]
    [InlineData("16", "16")]
    public async Task UpdateSchedule_MinuteEnvironment_Pass(int expected, int minutes)
    {
        Environment.SetEnvironmentVariable("LP_UPDATE_MINUTES", minutes.ToString());
        var create = new PackageUpdateScheduled(_crazyReport, _panelUpdateService, _eventBus);
        var expectedTimespan = new TimeSpan(0, 0, expected, 0);
        Assert.Equal(expectedTimespan, create._configuredTiming);
    }



    [Fact]
    public async Task UpdateSchedule_PanelVersion_LatestLowerSelfCancel_Pass()
    {
        _panelUpdateService.BlockedByPlugin.Returns(false);
        var version = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
        var latestVersion = new NuGetVersion(version.Major, version.Minor - 1, version.Patch, version.Release, version.Metadata);
        _panelUpdateService.GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken)).Returns(latestVersion);
        await _packageUpdateScheduled.Exec(TestContext.Current.CancellationToken);
        await _panelUpdateService.Received().GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken));
        await _panelUpdateService.DidNotReceive().CheckPluginsAsync(Arg.Is(TestContext.Current.CancellationToken));

    }


    [Fact]
    public async Task UpdateSchedule_PanelVersion_LatestSameCancel_Pass()
    {
        _panelUpdateService.BlockedByPlugin.Returns(false);
        var version = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
        _panelUpdateService.GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken)).Returns(version);
        await _packageUpdateScheduled.Exec(TestContext.Current.CancellationToken);
        await _panelUpdateService.Received().GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken));
        await _panelUpdateService.DidNotReceive().CheckPluginsAsync(Arg.Is(TestContext.Current.CancellationToken));

    }


    [Fact]
    public async Task UpdateSchedule_BlockedByPluginPending_Pass()
    {
        _panelUpdateService.BlockedByPlugin.Returns(true);
        await _packageUpdateScheduled.Exec(TestContext.Current.CancellationToken);
        await _panelUpdateService.DidNotReceive().GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken));
        await _panelUpdateService.DidNotReceive().CheckPluginsAsync(Arg.Is(TestContext.Current.CancellationToken));

    }


    [Fact]
    public async Task UpdateSchedule_BlockedByCompatibilityPending_Pass()
    {
        var version = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
        var currentVersion = new NuGetVersion(version.Major, version.Minor + 1, version.Patch, version.Release, version.Metadata);
        _panelUpdateService.GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken)).Returns(currentVersion);
        _panelUpdateService.AwaitingForPluginCompatibilityUpdate.Returns(true);
        await _packageUpdateScheduled.Exec(TestContext.Current.CancellationToken);
        await _panelUpdateService.Received().GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken));
        await _panelUpdateService.Received().CheckPluginsAsync(Arg.Is(TestContext.Current.CancellationToken));

    }


    [Fact]
    public async Task UpdateSchedule_BlockByInstallingUpdate_Pass()
    {
        var version = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
        var latestVersion = new NuGetVersion(version.Major, version.Minor + 1, version.Patch, version.Release, version.Metadata);
        _panelUpdateService.GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken)).Returns(latestVersion);
        _panelUpdateService.BlockedByPlugin.Returns(false);
        _panelUpdateService.AwaitingForPluginCompatibilityUpdate.Returns(false);
        _panelUpdateService.Downloaded.Returns(latestVersion);
        await _packageUpdateScheduled.Exec(TestContext.Current.CancellationToken);
        await _panelUpdateService.Received().GetLatestVersion(Arg.Is(TestContext.Current.CancellationToken));
        await _panelUpdateService.Received().CheckPluginsAsync(Arg.Is(TestContext.Current.CancellationToken));
        await _panelUpdateService.Received().StageUpdateAsync(Arg.Is(TestContext.Current.CancellationToken));

    }



}
