using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation.Layout.Menu.Pulses.States;

public record MainMenuState : IStateFeature
{
    public IReadOnlyCollection<MenuElementEntity> MenuElements { get; init; } = new List<MenuElementEntity>().AsReadOnly();
    public bool IsLoading { get; init; }
}
