using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Domain.UI.Dashboard.Entites;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Pages.Dashboard.Pulses.States;

public record DashboardState : IStateFeature
{
    public bool IsLoading { get; init; }
    public IReadOnlyCollection<EngineBusMsgResponseWithData<WidgetElementEntity>>? Widgets { get; init; } = new List<EngineBusMsgResponseWithData<WidgetElementEntity>>().AsReadOnly();
}
