using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Engine.Keys.UI;

namespace LunaticPanel.Plugin.Test;

[EngineBusId(MainMenuKeys.UI.GetElements)]
public class MenuBusTest : IEngineBusHandler
{
    private readonly MyService _myService;

    public MenuBusTest(MyService myService)
    {
        _myService = myService;
    }
    public Task<EngineBusResponse> HandleAsync(IEngineBusMessage engineBusMessage)
    {
        return Task.FromResult(new EngineBusResponse(typeof(Menu), new MenuElementResponse() { Position = 10 }));
    }
}
