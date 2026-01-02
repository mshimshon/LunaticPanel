using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.Home.CQRS.Queries;

public record FetchDashboardWidgetsQuery : IRequest<List<WidgetElementEntity>?>
{
}
