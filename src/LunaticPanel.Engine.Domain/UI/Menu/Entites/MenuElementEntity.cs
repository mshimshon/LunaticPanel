using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Domain.UI.Menu.Entites;

public record MenuElementEntity
{
    public int Position { get; set; }
    public RenderFragment Render { get; init; } = default!;
}
