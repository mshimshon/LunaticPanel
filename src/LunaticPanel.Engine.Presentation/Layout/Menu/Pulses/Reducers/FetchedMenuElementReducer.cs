using LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.Actions;
using LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.Reducers;

public class FetchedMenuElementReducer : IReducer<MainMenuState, FetchedMenuElementsAction>
{
    public Task<MainMenuState> ReduceAsync(MainMenuState state, FetchedMenuElementsAction action)
        => Task.FromResult(state with { IsLoading = false, MenuElements = action.MenuElements.AsReadOnly() });
}
