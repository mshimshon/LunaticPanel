using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.Actions;
using LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.States;
using LunaticPanel.Engine.Web.Services.PanelControl;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Pages.Dashboard;

public class DashboardViewModel : IDisposable
{
    private readonly IStatePulse _statePulse;
    private readonly IDispatcher _dispatcher;
    private readonly PanelControl _panelControl;

    public Func<Task>? SpreadChanges { get; set; }
    private bool _loading = false;
    private bool _disposedValue;

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
    public DashboardViewModel(IStatePulse statePulse, IDispatcher dispatcher, PanelControl panelControl)
    {
        _statePulse = statePulse;
        _dispatcher = dispatcher;
        _panelControl = panelControl;
        _panelControl.DashboardStateHasChanged += OnUpdate;
    }


    public async Task LoadAsync()
    {
        await _dispatcher.Prepare<FetchDashboardWidgetAction>().DispatchAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _panelControl.DashboardStateHasChanged -= OnUpdate;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
