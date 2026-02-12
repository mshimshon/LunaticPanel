using LunaticPanel.Engine.Domain.UI.Dashboard.Enums;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Domain.UI.Dashboard.Entites;

public sealed record WidgetElementEntity
{
    public int Position { get; init; }
    public WidgetSize Size { get; init; }
    public RenderFragment? Render { get; init; }
    public Type? ComponentType { get; init; }
}
