using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Core.UI;
using Microsoft.AspNetCore.Components;

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
        RenderFragment fragment = builder =>
        {
            builder.OpenComponent<Menu>(0);
            builder.CloseComponent();
        };

        return Task.FromResult(new EngineBusResponse(fragment, new MenuElementResponse() { Position = 10 }));
    }
}
