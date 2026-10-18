using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Engine.Keys.System;

namespace LunaticPanel.Engine.Web.Hooks.Events;

[EventBusKey(PanelKeys.Events.PanelShutdown)]
internal class ShutdownPanelEvent : IEventBusHandler
{
    private readonly IPanelControl _panelControl;

    public ShutdownPanelEvent(IPanelControl panelControl)
    {
        _panelControl = panelControl;
    }
    public Task HandleAsync(IEventBusMessage evt)
    {
        _panelControl.Shutdown();
        return Task.CompletedTask;
    }
}
