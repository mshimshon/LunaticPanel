using LunaticPanel.Engine.Mediator.Payloads.Responses;
using LunaticPanel.Engine.Models.Menu;

namespace LunaticPanel.Engine.Mediator.Payloads.Responses.Mapping;

public static class MenuElementMappingExt
{
    internal static MenuElementModel ToModel(this MenuElementResponse data)
    {
        return new() { Position = data.Position };
    }
}
