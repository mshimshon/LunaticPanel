using CoreMap;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Application.UI.Home.CQRS.Queries.Dto;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.Home.CQRS.Queries.Handlers;

internal class FetchDashboardWidgetsHandler : IRequestHandler<FetchDashboardWidgetsQuery, List<EngineBusMsgResponseWithData<WidgetElementEntity>>?>
{
    private readonly IEngineBus _engineBus;
    private readonly ICoreMap _coreMap;

    public FetchDashboardWidgetsHandler(IEngineBus engineBus, ICoreMap coreMap)
    {
        _engineBus = engineBus;
        _coreMap = coreMap;
    }
    public async Task<List<EngineBusMsgResponseWithData<WidgetElementEntity>>?> Handle(FetchDashboardWidgetsQuery request, CancellationToken cancellationToken)
    {
        List<EngineBusMsgResponseWithData<WidgetElementEntity>> result = new List<EngineBusMsgResponseWithData<WidgetElementEntity>>();
        try
        {
            var responses =
                await _engineBus
                .Execute(DashboardKeys.UI.GetWidgets)
                .ReadWithData((response) => _coreMap.Map((response.Data?.GetDataAs<WidgetElementResponse>() ?? new())).To<WidgetElementEntity>(),
                p => p);
            result = responses.OrderBy(p => p.Data.Position).ToList();
            Console.WriteLine($"FetchDashboardWidgetsHandler::Handle Count={result.Count}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"FetchDashboardWidgetsHandler::Handle = {ex.Message}");
        }
        return result;
    }
}
