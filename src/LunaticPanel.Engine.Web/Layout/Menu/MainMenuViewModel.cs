using LunaticPanel.Engine.Web.Layout.Menu.Pulses.Actions;
using LunaticPanel.Engine.Web.Layout.Menu.Pulses.States;
using LunaticPanel.Engine.Web.Services.PanelControl;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Layout.Menu;

public class MainMenuViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IDispatcher _dispatcher;
    private readonly PanelControl _panelControl;
    public Guid PanelId => _panelControl.Id;
    public Func<Task>? SpreadChanges { get; set; }
    private bool _loading = true;
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
    public MainMenuState State => _statePulse.StateOf<MainMenuState>(() => this, OnUpdate);
    public async Task OnUpdate() => SpreadChanges?.Invoke();
    public MainMenuViewModel(IStatePulse statePulse, IDispatcher dispatcher, PanelControl panelControl)
    {
        _statePulse = statePulse;
        _dispatcher = dispatcher;
        _panelControl = panelControl;
        panelControl.MenuStateHasChanged += OnUpdate;
    }

    public async Task LoadAsync()
    {
        await GetPluginMenuItems();
        Loading = false;
    }

    public async Task GetPluginMenuItems()
    {
        try
        {
            await _dispatcher.Prepare<FetchMenuElementsAction>().DispatchAsync();
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _panelControl.MenuStateHasChanged -= OnUpdate;
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
