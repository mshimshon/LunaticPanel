using CoreMap;
using LunaticPanel.Engine.Application.UI.MainMenu.Queries.Dto.Responses;
using LunaticPanel.Engine.Domain.Menu;

namespace LunaticPanel.Engine.Application.UI.MainMenu.Queries.Dto.Responses.Mapping;

public class MenuElementToMenuEntity : ICoreMapHandler<MenuElementResponse, MenuElementEntity>
{
    MenuElementEntity ICoreMapHandler<MenuElementResponse, MenuElementEntity>.Handler(MenuElementResponse data, ICoreMap alsoMap)
        => new()
        {
            Position = data.Position,
        };
}
