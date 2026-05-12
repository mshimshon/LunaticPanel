using CoreMap;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using LunaticPanel.Engine.Domain.UI.Dashboard.Enums;

namespace LunaticPanel.Engine.Application.UI.Home.CQRS.Queries.Dto.Mapping;

internal class WidgetElementResponseToWidgetElementEntity : ICoreMapHandler<WidgetElementResponse, WidgetElementEntity>
{
    public WidgetElementEntity Handler(WidgetElementResponse data, ICoreMap alsoMap)
        => new WidgetElementEntity()
        {
            Position = data.Position,
            Size = Enum.IsDefined(typeof(WidgetSize), data.Size) ? (WidgetSize)data.Size : WidgetSize.Twelve,
        };
}
