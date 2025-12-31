using LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.Actions;
using LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.Reducers;

public class FetchMenuElementReducer : IReducer<MainMenuState, FetchMenuElementsAction>
{
    public Task<MainMenuState> ReduceAsync(MainMenuState state, FetchMenuElementsAction action)
        => Task.FromResult(state with { IsLoading = true });
}
