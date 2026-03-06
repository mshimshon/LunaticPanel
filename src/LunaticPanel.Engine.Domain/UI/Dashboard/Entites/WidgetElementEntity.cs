using LunaticPanel.Engine.Domain.UI.Dashboard.Enums;

namespace LunaticPanel.Engine.Domain.UI.Dashboard.Entites;

public sealed record WidgetElementEntity
{
    public int Position { get; init; }
    public WidgetSize Size { get; init; }
}
