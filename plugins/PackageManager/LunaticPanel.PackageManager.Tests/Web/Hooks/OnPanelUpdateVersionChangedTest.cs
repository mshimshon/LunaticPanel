using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.PackageManager.Hooks.Events;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using NSubstitute;
using NuGet.Versioning;
using StatePulse.Net;
using System;
using System.Reflection;

namespace LunaticPanel.PackageManager.Tests.Web.Hooks;

public class OnPanelUpdateVersionChangedTest
{
    private readonly IDispatcherPrepper<UpdatePanelStateDoneAction> _dispatcherPrepper;
    private readonly IStateAccessor<PanelUpdateState> _panelUpdateState;
    private readonly IEventBusMessage _eventBusMessage;
    private readonly IDispatcher _dispatcher;
    private readonly OnPanelUpdateVersionChanged _onPanelUpdateVersionChanged;
    private readonly NuGetVersion _panelCurrent;
    public OnPanelUpdateVersionChangedTest()
    {

        _eventBusMessage = Substitute.For<IEventBusMessage>();
        _dispatcher = Substitute.For<IDispatcher>();
        _panelUpdateState = Substitute.For<IStateAccessor<PanelUpdateState>>();
        _panelUpdateState.State.Returns(new PanelUpdateState());
        _onPanelUpdateVersionChanged = new OnPanelUpdateVersionChanged(_dispatcher, _panelUpdateState);

        _dispatcherPrepper = Substitute.For<IDispatcherPrepper<UpdatePanelStateDoneAction>>();
        _dispatcherPrepper.ActionInstance.Returns(new UpdatePanelStateDoneAction());

        _dispatcher.Prepare<UpdatePanelStateDoneAction>().Returns(_dispatcherPrepper);
        _panelCurrent = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
    }

    [Fact]
    public async Task HandleAsync_ShouldSelfCancel_PanelVersionNotMet_Pass()
    {
        _eventBusMessage.GetData().Returns(new BusMessageData(_panelCurrent.ToString()));
        await _onPanelUpdateVersionChanged.HandleAsync(_eventBusMessage);
        _dispatcher.DidNotReceive().Prepare<UpdatePanelStateDoneAction>();
    }

    [Fact]
    public async Task HandleAsync_ShouldSpreadViaStatePulse_PanelVersionNotMet_Pass()
    {
        var latest = new NuGetVersion(_panelCurrent.Major, _panelCurrent.Minor + 1, _panelCurrent.Patch, _panelCurrent.Release, _panelCurrent.Metadata);
        _eventBusMessage.GetData().Returns(new BusMessageData(latest.ToString()));
        await _onPanelUpdateVersionChanged.HandleAsync(_eventBusMessage);

    }



}
