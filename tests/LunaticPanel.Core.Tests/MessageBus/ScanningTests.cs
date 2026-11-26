using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Messaging;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Keys.UI;
using System;

namespace LunaticPanel.Core.Tests.MessageBus;

public class ScanningTests
{
    [EngineBusKey(DashboardKeys.UI.GET_WIDGETS)]
    [EngineBusKey(MainMenuKeys.UI.GET_ELEMENTS)]
    public class EngineBusClass : IEngineBusHandler
    {
        public Task<EngineBusResponse> HandleAsync(IEngineBusMessage engineBusMessage) => throw new NotImplementedException();
    }
    [Fact]
    public void ScanEngineBus()
    {
        var result = BusScannerExt.ScanBusHandlers((s) => { }, typeof(EngineBusClass));
        Assert.Contains(result, item => item.Key == DashboardKeys.UI.GET_WIDGETS);
    }


    [Fact]
    public void ScanEngineBus_MultipleEntry()
    {
        var expectedType = typeof(EngineBusClass);
        var result = BusScannerExt.ScanBusHandlers((s) => { }, expectedType);
        Assert.Contains(result, item => item.Key == DashboardKeys.UI.GET_WIDGETS && item.HandlerType == expectedType);
        Assert.Contains(result, item => item.Key == MainMenuKeys.UI.GET_ELEMENTS && item.HandlerType == expectedType);
    }

    [EventBusKey(PanelKeys.Events.CHECK_PANEL_UPDATE)]
    [EventBusKey(PanelKeys.Events.ON_PANEL_UPDATE_AVAILABLE)]
    public class EventBusClass : IEventBusHandler
    {
        public Task HandleAsync(IEventBusMessage evt) => throw new NotImplementedException();
    }
    [Fact]
    public void EventBus()
    {
        var result = BusScannerExt.ScanBusHandlers((s) => { }, typeof(EngineBusClass));
        Assert.Contains(result, item => item.Key == DashboardKeys.UI.GET_WIDGETS);
    }


    [Fact]
    public void EventBus_MultipleEntry()
    {
        var expectedType = typeof(EventBusClass);
        var result = BusScannerExt.ScanBusHandlers((s) => { }, expectedType);
        Assert.Contains(result, item => item.Key == PanelKeys.Events.CHECK_PANEL_UPDATE && item.HandlerType == expectedType);
        Assert.Contains(result, item => item.Key == PanelKeys.Events.ON_PANEL_UPDATE_AVAILABLE && item.HandlerType == expectedType);
    }


    [QueryBusKey(PanelKeys.Events.CHECK_PANEL_UPDATE)]
    public class WrongAttributeHandler : IEventBusHandler
    {
        public Task HandleAsync(IEventBusMessage evt) => throw new NotImplementedException();
    }
    public class NoAttributeHandler : IEventBusHandler
    {
        public Task HandleAsync(IEventBusMessage evt) => throw new NotImplementedException();
    }


    [Fact]
    public async Task BusScanner_ShouldThrowInvalid_WrongAttributeMatch()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
        {
            BusScannerExt.ScanBusHandlers((s) => { }, typeof(WrongAttributeHandler));
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task BusScanner_ShouldThrowInvalid_NoAttribute()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
        {
            BusScannerExt.ScanBusHandlers((s) => { }, typeof(NoAttributeHandler));
            return Task.CompletedTask;
        });
    }

    [QueryBusKey(DashboardKeys.UI.GET_WIDGETS)]
    public class QueryBusClass : IQueryBusHandler
    {
        public Task<QueryBusMessageResponse> HandleAsync(IQueryBusMessage qry) => throw new NotImplementedException();
    }
    [Fact]
    public void QueryBusScan()
    {
        var expectedType = typeof(QueryBusClass);
        var result = BusScannerExt.ScanBusHandlers((s) => { }, expectedType);
        Assert.Contains(result, item => item.Key == DashboardKeys.UI.GET_WIDGETS && item.HandlerType == expectedType);
        Assert.True(result.Where(p => p.BusType == EBusType.QueryBus).ToList().Count == 1);
    }


    [EventScheduledBusKey(DashboardKeys.UI.GET_WIDGETS)]
    public class EventScheduledBusClass : IEventScheduledBusHandler
    {
        public EventScheduledBusMessageData DueToExecute(IEventScheduledBusMessage msg, CancellationToken ct = default) => throw new NotImplementedException();
    }

    [Fact]
    public void EventScheduledBusScan()
    {
        var expectedType = typeof(EventScheduledBusClass);
        var result = BusScannerExt.ScanBusHandlers((s) => { }, expectedType);
        Assert.Contains(result, item => item.Key == DashboardKeys.UI.GET_WIDGETS && item.HandlerType == expectedType);
        Assert.True(result.Where(p => p.BusType == EBusType.EventScheduledBus).ToList().Count == 1);
    }




}
