using CoreMap;
using LunaticPanel.Core.Extenstions;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Application.UI.Home.Queries.Dto;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using MedihatR;

namespace LunaticPanel.Engine.Application.UI.Home.Queries.Handlers;

internal class FetchDashboardWidgetsHandler : IRequestHandler<FetchDashboardWidgetsQuery, List<WidgetElementEntity>>
{
    private readonly IEngineBus _engineBus;
    private readonly ICoreMap _coreMap;

    public FetchDashboardWidgetsHandler(IEngineBus engineBus, ICoreMap coreMap)
    {
        _engineBus = engineBus;
        _coreMap = coreMap;
    }
    public async Task<List<WidgetElementEntity>> Handle(FetchDashboardWidgetsQuery request, CancellationToken cancellationToken)
    {
        List<WidgetElementEntity> result = new();
        try
        {
            var responses = await _engineBus
                .Execute(DashboardKeys.Queries.GetWidgets)
                .ReadWithData(msg => _coreMap.Map(msg.GetDataAs<WidgetElementResponse>()!).To<WidgetElementEntity>());
            foreach (var item in responses)
                try
                {
                    result.Add(item.Data with { Render = item.Render });
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
