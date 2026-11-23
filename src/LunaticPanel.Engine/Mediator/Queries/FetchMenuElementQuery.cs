using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Models.Menu;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Engine.Mediator.Queries;

public record FetchMenuElementQuery() : IRequest<List<EngineBusMsgResponseWithData<MenuElementModel>>>
{
}
