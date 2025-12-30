using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Presentation.Layout.Menu.Models;

public record MenuElementModel
{
    public int Position { get; set; }
    public RenderFragment Render { get; init; } = default!;
}
