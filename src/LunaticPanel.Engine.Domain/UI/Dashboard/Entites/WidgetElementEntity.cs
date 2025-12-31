using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Domain.UI.Dashboard.Entites;

public record WidgetElementEntity
{
    public int Position { get; set; }
    public RenderFragment Render { get; init; } = default!;
}
