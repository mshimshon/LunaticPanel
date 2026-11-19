using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using StatePulse.Net;

namespace LunaticPanel.Engine.Layout.Menu.Pulses.Actions;

public record FetchedMenuElementsAction() : IAction
{
    public List<EngineBusMsgResponseWithData<MenuElementEntity>> MenuElements { get; set; } = default!;
}
