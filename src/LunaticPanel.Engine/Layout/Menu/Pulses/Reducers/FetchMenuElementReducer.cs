using LunaticPanel.Engine.Web.Layout.Menu.Pulses.Actions;
using LunaticPanel.Engine.Web.Layout.Menu.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Layout.Menu.Pulses.Reducers;

public class FetchMenuElementReducer : IReducer<MainMenuState, FetchMenuElementsAction>
{
    public MainMenuState Reduce(MainMenuState state, FetchMenuElementsAction action)
        => state with
        {
            IsLoading = true
        };
}
