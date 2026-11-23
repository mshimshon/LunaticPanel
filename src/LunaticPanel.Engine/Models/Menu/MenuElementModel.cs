using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace LunaticPanel.Engine.Models.Menu;

public sealed record MenuElementModel
{

    public int Position { get; init; }
    [JsonIgnore]
    public RenderFragment? Render { get; }
    [JsonIgnore]
    public Type? ComponentType { get; }

    public MenuElementModel() { }
    public MenuElementModel(RenderFragment render)
    {
        Render = render;
    }

    public MenuElementModel(Type componentType)
    {
        ComponentType = componentType;
    }
}
