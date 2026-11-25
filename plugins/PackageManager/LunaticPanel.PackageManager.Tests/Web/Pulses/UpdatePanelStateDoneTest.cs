using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.PackageManager.Hooks.Events;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Tests.Web.Pulses.Utils;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NuGet.Versioning;
using StatePulse.Net;
using System;
using System.Reflection;

namespace LunaticPanel.PackageManager.Tests.Web.Pulses;

public class UpdatePanelStateDoneTest : PulsesBaseTests
{
    private readonly IEventBus _eventBus;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<PanelUpdateState> _panelUpdateState;
    private readonly IServiceProvider _sp;
    private readonly NuGetVersion _panelCurrent;
    public UpdatePanelStateDoneTest()
    {

        _eventBus = Substitute.For<IEventBus>();
        _dispatcher = Substitute.For<IDispatcher>();
        _dispatcher.CancelToken.Returns(TestContext.Current.CancellationToken);
        _services.AddScoped((sp) => _eventBus);

        _sp = _services.BuildServiceProvider();
        _dispatcher = _sp.GetRequiredService<IDispatcher>();
        _panelUpdateState = _sp.GetRequiredService<IStateAccessor<PanelUpdateState>>();
        _panelCurrent = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
    }


    [Fact]
    public async Task HandleAsync_ShouldSpreadViaStatePulse_PanelVersionNotMet_Pass()
    {
        var latest = new NuGetVersion(_panelCurrent.Major, _panelCurrent.Minor + 1, _panelCurrent.Patch, _panelCurrent.Release, _panelCurrent.Metadata);
        await _dispatcher.Prepare<UpdatePanelStateDoneAction>()
            .With(p => p.Found, latest)
            .With(p => p.Downloaded, latest)
            .With(p => p.Staged, latest).Await()
            .DispatchAsync(TestContext.Current.CancellationToken);
        Assert.Equal(_panelUpdateState.State.AvailableVersion, latest);
        Assert.Equal(_panelUpdateState.State.DownloadedVersion, latest);
        Assert.Equal(_panelUpdateState.State.StagedVersion, latest);
    }



}
