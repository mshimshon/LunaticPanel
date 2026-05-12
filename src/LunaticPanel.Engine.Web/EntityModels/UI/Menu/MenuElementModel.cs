using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Web.EntityModels.UI.Menu;

internal sealed record MenuElementModel
{

    public int Position { get; init; }
    public RenderFragment? Render { get; }
    public Type? ComponentType { get; }

    public MenuElementModel(RenderFragment render)
    {
        Render = render;
    }

    public MenuElementModel(Type componentType)
    {
        ComponentType = componentType;
    }
}
