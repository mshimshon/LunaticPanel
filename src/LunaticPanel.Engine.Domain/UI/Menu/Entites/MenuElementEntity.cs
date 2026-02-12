using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Domain.UI.Menu.Entites;

public sealed record MenuElementEntity
{
    public int Position { get; init; }

    public RenderFragment? Render { get; init; }
    public Type? ComponentType { get; init; }
}
