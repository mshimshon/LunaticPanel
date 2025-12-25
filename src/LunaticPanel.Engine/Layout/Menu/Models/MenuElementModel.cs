using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Layout.Menu.Models;

public record MenuElementModel
{
    public int Position { get; set; }
    public RenderFragment Render { get; init; } = default!;
}
