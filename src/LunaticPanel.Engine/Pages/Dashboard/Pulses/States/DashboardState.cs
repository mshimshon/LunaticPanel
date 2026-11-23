using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Models.Dashboard;
using StatePulse.Net;

namespace LunaticPanel.Engine.Pages.Dashboard.Pulses.States;

public record DashboardState : IStateFeature
{
    public bool IsLoading { get; init; }
    public IReadOnlyCollection<EngineBusMsgResponseWithData<WidgetElementModel>>? Widgets { get; init; } = new List<EngineBusMsgResponseWithData<WidgetElementModel>>().AsReadOnly();
}
