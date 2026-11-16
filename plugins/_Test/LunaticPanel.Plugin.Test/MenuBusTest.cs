using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.UI;

namespace LunaticPanel.Plugin.Test;

[EngineBusKey(MainMenuKeys.UI.GET_ELEMENTS)]
public class MenuBusTest : IEngineBusHandler
{
    private readonly MyService _myService;
    private readonly IEngineBus _engineBus;

    public MenuBusTest(MyService myService, IEngineBus engineBus)
    {
        _myService = myService;
        _engineBus = engineBus;
    }
    public Task<EngineBusResponse> HandleAsync(IEngineBusMessage engineBusMessage)
    {
        _ = _engineBus.Execute(DashboardKeys.UI.GET_WIDGETS);
        _ = _engineBus.Execute("");
        return Task.FromResult(new EngineBusResponse(typeof(Menu), new MenuElementResponse() { Position = 10 }));
    }
}
