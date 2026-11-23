using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Models.Menu;
using StatePulse.Net;

namespace LunaticPanel.Engine.Layout.Menu.Pulses.Actions;

public record FetchedMenuElementsAction() : IAction
{
    public List<EngineBusMsgResponseWithData<MenuElementModel>> MenuElements { get; set; } = default!;
}
