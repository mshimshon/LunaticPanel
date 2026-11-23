using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Models.Dashboard;
using LunaticPanel.Engine.Models.Menu;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Engine.Mediator.Queries;

public record FetchDashboardWidgetsQuery : IRequest<List<EngineBusMsgResponseWithData<WidgetElementModel>>?>
{
}
