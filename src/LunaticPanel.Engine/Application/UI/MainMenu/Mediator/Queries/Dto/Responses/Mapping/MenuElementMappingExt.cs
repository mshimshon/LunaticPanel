using LunaticPanel.Engine.Application.UI.MainMenu.Mediator.Queries.Dto.Responses;
using LunaticPanel.Engine.Domain.UI.Menu.Entites;

namespace LunaticPanel.Engine.Application.UI.MainMenu.Mediator.Queries.Dto.Responses.Mapping;

public static class MenuElementMappingExt
{
    internal static MenuElementEntity ToEntity(this MenuElementResponse data)
    {
        return new() { Position = data.Position };
    }
}
