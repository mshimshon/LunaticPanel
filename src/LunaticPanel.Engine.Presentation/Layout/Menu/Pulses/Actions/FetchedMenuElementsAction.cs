using LunaticPanel.Engine.Domain.Menu;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.Actions;

public record FetchedMenuElementsAction() : IAction
{
    public List<MenuElementEntity> MenuElements { get; set; } = default!;
}
