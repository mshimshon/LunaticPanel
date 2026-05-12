using CoreMap;
using LunaticPanel.Engine.Domain.UI.Menu.Entites;

namespace LunaticPanel.Engine.Application.UI.MainMenu.CQRS.Queries.Dto.Responses.Mapping;

public class MenuElementToMenuEntity : ICoreMapHandler<MenuElementResponse, MenuElementEntity>
{
    MenuElementEntity ICoreMapHandler<MenuElementResponse, MenuElementEntity>.Handler(MenuElementResponse data, ICoreMap alsoMap)
    {

        return new() { Position = data.Position };
    }
}
