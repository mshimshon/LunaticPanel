using LunaticPanel.Core;

namespace LunaticPanel.Engine.Services.Messaging.EngineBus;

public record EngineBusHandlerDescriptor(
        string Id,
        Type HandlerType,
        IPlugin? Plugin
    )
{
}
