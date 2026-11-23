using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Models.Menu;
using StatePulse.Net;

namespace LunaticPanel.Engine.Layout.Menu.Pulses.States;

public record MainMenuState : IStateFeature
{
    public IReadOnlyCollection<EngineBusMsgResponseWithData<MenuElementModel>> MenuElements { get; init; } = new List<EngineBusMsgResponseWithData<MenuElementModel>>().AsReadOnly();
    public bool IsLoading { get; init; }
}
