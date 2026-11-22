using LunaticPanel.Engine.Application.UI.Home.Mediator.Queries.Dto;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using LunaticPanel.Engine.Domain.UI.Dashboard.Enums;

namespace LunaticPanel.Engine.Application.UI.Home.Mediator.Queries.Dto.Mapping;

public static class WidgetElementMappingExt
{
    internal static WidgetElementEntity ToEntity(this WidgetElementResponse data)
        => new WidgetElementEntity()
        {
            Position = data.Position,
            Size = Enum.IsDefined(typeof(WidgetSize), data.Size) ? (WidgetSize)data.Size : WidgetSize.Twelve,
        };
}
