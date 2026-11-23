using LunaticPanel.Engine.Models.Dashboard.Enums;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Models.Dashboard;

public sealed record WidgetElementModel
{
    public int Position { get; init; }
    public WidgetSize Size { get; init; }

    public RenderFragment? Render { get; }
    public Type? ComponentType { get; }
    public WidgetElementModel() { }

    public WidgetElementModel(RenderFragment render)
    {
        Render = render;
    }

    public WidgetElementModel(Type componentType)
    {
        ComponentType = componentType;
    }
}
