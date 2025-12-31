using CoreMap;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;

namespace LunaticPanel.Engine.Application.UI.Home.Queries.Dto.Mapping;

internal class WidgetElementResponseToWidgetElementEntity : ICoreMapHandler<WidgetElementResponse, WidgetElementEntity>
{
    public WidgetElementEntity Handler(WidgetElementResponse data, ICoreMap alsoMap)
        => new() { Position = data.Position };
}
