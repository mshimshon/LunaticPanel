using LunaticPanel.Core.Messaging.EngineBus;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Plugin.Test;

[EngineBusId("Engine.Layout.Menu")]
public class MenuBusTest : IEngineBusHandler
{
    public Task<EngineBusResponse> HandleAsync(IEngineBusMessage engineBusMessage)
    {
        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, Guid.NewGuid().ToString());
            builder.CloseElement();
        };

        return Task.FromResult(new EngineBusResponse(fragment, new MenuElementResponse() { Position = 10 }));
    }
}
