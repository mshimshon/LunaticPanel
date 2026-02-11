using CoreMap;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Application.UI.Home.CQRS.Queries.Dto;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.Home.CQRS.Queries.Handlers;

internal class FetchDashboardWidgetsHandler : IRequestHandler<FetchDashboardWidgetsQuery, List<WidgetElementEntity>?>
{
    private readonly IEngineBus _engineBus;
    private readonly ICoreMap _coreMap;

    public FetchDashboardWidgetsHandler(IEngineBus engineBus, ICoreMap coreMap)
    {
        _engineBus = engineBus;
        _coreMap = coreMap;
    }
    public async Task<List<WidgetElementEntity>?> Handle(FetchDashboardWidgetsQuery request, CancellationToken cancellationToken)
    {
        List<WidgetElementEntity> result = new();
        try
        {
            var responses =
                await _engineBus
                .Execute(DashboardKeys.UI.GetWidgets)
                .ReadWithData(msg => _coreMap.Map(msg?.GetDataAs<WidgetElementResponse>() ?? new()).To<WidgetElementEntity>());
            foreach (EngineBusMsgResponseWithData<WidgetElementEntity> item in responses)
                try
                {
                    result.Add(item.Data with
                    {
                        Render = item.Render,
                        VisibilityCondition = item.VisibilityCondition
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return result.OrderBy(p => p.Position).ToList();
    }
}
