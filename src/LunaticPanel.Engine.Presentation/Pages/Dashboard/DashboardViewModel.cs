using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.Actions;
using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Pages.Dashboard;

public class DashboardViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IDispatcher _dispatcher;

    public Func<Task>? SpreadChanges { get; set; }
    private bool _loading = false;
    public bool Loading
    {
        get => _loading || State.IsLoading;
        set
        {
            bool refresh = value != _loading;
            _loading = value;
            if (refresh)
                _ = SpreadChanges?.Invoke();
        }
    }
    public DashboardState State => _statePulse.StateOf<DashboardState>(() => this, OnUpdate);
    public async Task OnUpdate() => SpreadChanges?.Invoke();
    public DashboardViewModel(IStatePulse statePulse, IDispatcher dispatcher)
    {
        _statePulse = statePulse;
        _dispatcher = dispatcher;
    }


    public async Task LoadAsync()
    {
        await _dispatcher.Prepare<FetchDashboardWidgetAction>().EffectsFirst().DispatchAsync();
    }
}
