using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.MainMenu.CQRS.Queries;

public record FetchMenuElementQuery() : IRequest<List<MenuElementEntity>>
{
}
