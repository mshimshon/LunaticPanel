using LunaticPanel.Engine.Domain.Menu;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.MainMenu.Queries;

public record FetchMenuElementQuery() : IRequest<List<MenuElementEntity>>
{
}
