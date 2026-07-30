using LunaticPanel.Engine.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Web.Pages.Debugging;

public partial class SchedulerDebug
{
    [Inject] private IEventSchedulerDiagnostic EventSchedulerDiagnostic { get; } = default!;

}
