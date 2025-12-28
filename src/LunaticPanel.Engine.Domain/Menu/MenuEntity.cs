using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Domain.Menu;

public record MenuEntity
{
    public int Position { get; set; }
    public RenderFragment Render { get; init; } = default!;
}
