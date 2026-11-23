using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.UI;
using LunaticPanel.Engine.Mediator.Payloads.Responses;
using LunaticPanel.Engine.Mediator.Payloads.Responses.Mapping;
using LunaticPanel.Engine.Mediator.Queries;
using LunaticPanel.Engine.Models.Dashboard;
using LunaticPanel.Engine.Models.Menu;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Engine.Mediator.Queries.Handlers;

internal class FetchDashboardWidgetsHandler : IRequestHandler<FetchDashboardWidgetsQuery, List<EngineBusMsgResponseWithData<WidgetElementModel>>?>
{
    private readonly IEngineBus _engineBus;

    public FetchDashboardWidgetsHandler(IEngineBus engineBus)
    {
        _engineBus = engineBus;
    }
    public async Task<List<EngineBusMsgResponseWithData<WidgetElementModel>>?> HandleAsync(FetchDashboardWidgetsQuery request, CancellationToken cancellationToken)
    {
        List<EngineBusMsgResponseWithData<WidgetElementModel>> result = new List<EngineBusMsgResponseWithData<WidgetElementModel>>();
        try
        {
            var responses =
                await _engineBus
                .Execute(DashboardKeys.UI.GET_WIDGETS)
                .ReadWithData((EngineBusResponse response) =>
                    (response.Data?.GetDataAs<WidgetElementResponse>() ?? new()).ToModel(),
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
