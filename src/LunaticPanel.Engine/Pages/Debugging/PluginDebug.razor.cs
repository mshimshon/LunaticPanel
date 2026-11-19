using LunaticPanel.Engine.Services;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Web.Pages.Debugging;

public partial class PluginDebug : ComponentBase
{
    [Inject] public IPluginRegistry PluginRegistry { get; set; } = default!;
    protected override void OnInitialized()
    {

    }
}
