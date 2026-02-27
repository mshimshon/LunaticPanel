using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Tools.EventScheduler;
using LunaticPanel.Core.Extensions;

namespace LunaticPanel.Engine.Infrastructure.Services;


internal class EventScheduler : IEventScheduler
{


    private PriorityQueue<EventScheduleTaskObject, DateTime> _queue = new();
    private List<Guid> _activeActions = new();
    private readonly CancellationTokenSource _cancelTokenSourceForLoop = new();
    private readonly IEventScheduledBus _eventScheduledBus;
    private readonly IEventScheduledBusExchange _eventScheduledBusExchange;
    private CancellationTokenSource _cancelTokenSourceForAwaitedSchedule = new();
    private readonly object _lock = new();

    public EventScheduler(IEventScheduledBus eventScheduledBus, IEventScheduledBusExchange eventScheduledBusExchange)
    {
        _eventScheduledBus = eventScheduledBus;
        _eventScheduledBusExchange = eventScheduledBusExchange;

    }

    public Guid Register(EventScheduleObject task, bool runNow)
    {

        if (!_eventScheduledBusExchange.AnyListenerFor(task.Id))
            throw new Exception("blah blah blah"); // TODO: FIX
        var nextId = Guid.NewGuid();
        var message = new EventScheduledBusMessage(new(task.Id));
        var schedule = new EventScheduleTaskObject(task.Id, task.InitialTime)
        {
            RunOnceOnly = task.RunOnceOnly,
            ScheduleId = message.GetMessageId(),
            Action = () => _eventScheduledBus.PublishAsync(message, _cancelTokenSourceForLoop.Token)
        };
        lock (_lock)
        {
            _activeActions.Add(schedule.ScheduleId);
            _queue.Enqueue(schedule, DateTime.UtcNow.Add(schedule.InitialTime));
            if (_queue.TryPeek(out var element, out DateTime priority))
            {
                if (element == schedule)
                    _cancelTokenSourceForAwaitedSchedule.Cancel();
            }
        }
        if (runNow)
            _ = schedule.Action();

        return schedule.ScheduleId;
    }

    public void UnRegister(Guid id)
    {
        lock (_lock)
        {
            _activeActions.Remove(id);

        }
    }

    public async Task Cycle()
    {
        do
        {

            _cancelTokenSourceForAwaitedSchedule = new CancellationTokenSource();
            var delay = Math.Max(50, 30000); // TODO: MAKE DELAY FOR SCHEDULE WAKE UP A SETTING
            if (!_queue.TryPeek(out var element, out DateTime priority))
            {
                await Task.Delay(delay, _cancelTokenSourceForAwaitedSchedule.Token);
                continue;
            }
            delay = Math.Max(50, (int)(priority - DateTime.UtcNow).TotalMilliseconds);
            await Task.Delay(delay, _cancelTokenSourceForAwaitedSchedule.Token);

            lock (_lock)
            {

                if (_cancelTokenSourceForAwaitedSchedule.Token.IsCancellationRequested) continue;
                _queue.Dequeue();
                if (!_activeActions.Contains(element.ScheduleId))
                    continue;
            }

            EventScheduledBusMessageResponse? result = await element.Action();
            if (result.Error != default || result.Data == default)
            {
                //TODO: LOG ERROR
                lock (_lock)
                    _queue.Enqueue(element, DateTime.UtcNow.Add(element.InitialTime));
                continue;
            }

            var data = result.Data;
            var elementToEnqueue = element;

            bool runningOnlyOnce = elementToEnqueue.RunOnceOnly;

            if (data.HasDisabledRunOnlyOnceForNext())
                runningOnlyOnce = false;

            if (runningOnlyOnce)
                continue;

            if (data.ForceReschedule != default && data.ForceReschedule == false)
                continue;

            if (data.NextRunOnlyOnce != default)
                elementToEnqueue = elementToEnqueue with
                {
                    RunOnceOnly = (bool)data.NextRunOnlyOnce!
                };

            if (data.NextTiming != default)
            {
                var newTiming = ((TimeSpan)data.NextTiming!).TotalMilliseconds < 50 ? TimeSpan.FromMilliseconds(50) : (TimeSpan)data.NextTiming;
                elementToEnqueue = new EventScheduleTaskObject(elementToEnqueue.Id, newTiming)
                {
                    Action = elementToEnqueue.Action,
                    RunOnceOnly = elementToEnqueue.RunOnceOnly,
                    ScheduleId = elementToEnqueue.ScheduleId
                };

            }

            var nextScheduleTime = DateTime.UtcNow.Add(elementToEnqueue.InitialTime);
            if (result.Data.NextRun != default)
                nextScheduleTime = (DateTime)result.Data.NextRun!;


            lock (_lock)
                _queue.Enqueue(elementToEnqueue, nextScheduleTime);

        } while (_cancelTokenSourceForLoop.Token.IsCancellationRequested);
    }
}



