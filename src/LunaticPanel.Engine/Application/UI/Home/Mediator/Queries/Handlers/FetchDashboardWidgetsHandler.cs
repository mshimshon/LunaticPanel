using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Application.UI.Home.Mediator.Queries;
using LunaticPanel.Engine.Application.UI.Home.Mediator.Queries.Dto;
using LunaticPanel.Engine.Application.UI.Home.Mediator.Queries.Dto.Mapping;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using LunaticPanel.Engine.Keys.UI;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Engine.Application.UI.Home.Mediator.Queries.Handlers;

internal class FetchDashboardWidgetsHandler : IRequestHandler<FetchDashboardWidgetsQuery, List<EngineBusMsgResponseWithData<WidgetElementEntity>>?>
{
    private readonly IEngineBus _engineBus;

    public FetchDashboardWidgetsHandler(IEngineBus engineBus)
    {
        _engineBus = engineBus;
    }
    public async Task<List<EngineBusMsgResponseWithData<WidgetElementEntity>>?> HandleAsync(FetchDashboardWidgetsQuery request, CancellationToken cancellationToken)
    {
        List<EngineBusMsgResponseWithData<WidgetElementEntity>> result = new List<EngineBusMsgResponseWithData<WidgetElementEntity>>();
        try
        { // _coreMap.MapTo<WidgetElementResponse, WidgetElementEntity>
            var responses =
                await _engineBus
                .Execute(DashboardKeys.UI.GET_WIDGETS)
                .ReadWithData((EngineBusResponse response) => (response.Data?.GetDataAs<WidgetElementResponse>() ?? new()).ToEntity(),
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
