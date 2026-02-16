using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Layout.Menu.Pulses.States;

public record MainMenuState : IStateFeature
{
    public IReadOnlyCollection<EngineBusMsgResponseWithData<MenuElementEntity>> MenuElements { get; init; } = new List<EngineBusMsgResponseWithData<MenuElementEntity>>().AsReadOnly();
    public bool IsLoading { get; init; }
}
