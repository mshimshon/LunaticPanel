using LunaticPanel.Engine.Layout.Menu.Pulses.Actions;
using LunaticPanel.Engine.Layout.Menu.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Layout.Menu.Pulses.Reducers;

public class FetchedMenuElementReducer : IReducer<MainMenuState, FetchedMenuElementsAction>
{
    public MainMenuState Reduce(MainMenuState state, FetchedMenuElementsAction action)
        => state with
        {
            IsLoading = false,
            MenuElements = action.MenuElements.AsReadOnly()
        };
}
