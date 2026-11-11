using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Engine.Application.UI.Home.Mediator.Queries;

public record FetchDashboardWidgetsQuery : IRequest<List<EngineBusMsgResponseWithData<WidgetElementEntity>>?>
{
}
