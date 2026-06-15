using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.UI;
using LunaticPanel.PackageManager.Shared;

namespace LunaticPanel.PackageManager.Hooks.UserInterface;

[EngineBusKey(MainMenuKeys.UI.GetElements)]
public class MenuMainPageElementHook : IEngineBusHandler
{

    public Task<EngineBusResponse> HandleAsync(IEngineBusMessage engineBusMessage)
        => engineBusMessage.ReplyWithTypeOf<Menu>();
}
