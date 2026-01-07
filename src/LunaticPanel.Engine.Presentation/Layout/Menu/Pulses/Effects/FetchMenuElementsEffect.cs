using LunaticPanel.Engine.Application.UI.MainMenu.CQRS.Queries;
using LunaticPanel.Engine.Web.Layout.Menu.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Layout.Menu.Pulses.Effects;

public class FetchMenuElementsEffect : IEffect<FetchMenuElementsAction>
{
    private readonly IMedihater _medihater;

    public FetchMenuElementsEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync(FetchMenuElementsAction action, IDispatcher dispatcher)
    {
        var result = await _medihater.Send(new FetchMenuElementQuery());
        await dispatcher.Prepare<FetchedMenuElementsAction>().With(p => p.MenuElements, result).DispatchAsync();
    }
}
