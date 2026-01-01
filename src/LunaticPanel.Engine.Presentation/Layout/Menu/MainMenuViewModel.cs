using LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.Actions;
using LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation.Layout.Menu;

public class MainMenuViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IDispatcher _dispatcher;

    public Func<Task>? SpreadChanges { get; set; }
    private bool _loading = true;
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
    public MainMenuViewModel(IStatePulse statePulse, IDispatcher dispatcher)
    {
        _statePulse = statePulse;
        _dispatcher = dispatcher;
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
}
