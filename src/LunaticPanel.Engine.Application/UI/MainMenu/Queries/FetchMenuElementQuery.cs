using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.MainMenu.Queries;

public record FetchMenuElementQuery() : IRequest<List<MenuElementEntity>>
{
}
