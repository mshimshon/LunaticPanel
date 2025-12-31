using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.Home.Queries;

public record FetchDashboardWidgetsQuery : IRequest<List<WidgetElementEntity>>
{
}
