using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Pulses.States;
using NuGet.Versioning;
using StatePulse.Net;
using System.Reflection;

namespace LunaticPanel.PackageManager.Hooks.Events;

[EventBusKey(PanelKeys.Events.ON_PANEL_UPDATE_AVAILABLE, CrossCircuitReceiver = EventBusSpreadType.SelfContained)]
internal class OnPanelUpdateVersionChanged : IEventBusHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<PanelUpdateState> _panelUpdateState;

    private NuGetVersion _currentPanelVersion;
    public OnPanelUpdateVersionChanged(IDispatcher dispatcher, IStateAccessor<PanelUpdateState> panelUpdateState)
    {
        _dispatcher = dispatcher;
        _panelUpdateState = panelUpdateState;

        _currentPanelVersion = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
    }
    public async Task HandleAsync(IEventBusMessage evt)
    {
        string version = await evt.ReadAs<string>();
        NuGetVersion incomingVersion = NuGetVersion.Parse(version);
        if (_currentPanelVersion >= incomingVersion) return;
        var action = new UpdatePanelStateDoneAction()
        {
            Found = incomingVersion,
            Downloaded = _panelUpdateState.State.DownloadedVersion,
            Staged = _panelUpdateState.State.StagedVersion
        };
        await _dispatcher.Prepared(action).DispatchAsync();
    }
}
